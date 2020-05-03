/* Please run these in logical chunks. It may not run correctly if you run the entire file at once.*/

/*===== Creating Database Tables =====*/
CREATE SCHEMA Framing;
GO

--Creates an entry for each physical computer, 
--the client-side app will keep the computers external ip up to date.
CREATE TABLE Framing.Computers (
	Id int NOT NULL IDENTITY PRIMARY KEY,
	ExternalIP varchar(20) NOT NULL,
	MachineName varchar(50) NOT NULL UNIQUE,
	CreatedDate DATETIME NULL,
	CreatedBy varchar(20) NULL,
	ModifiedDate DATETIME NULL,
	ModifiedBy varchar(20) NULL,
);
GO

--Creates a table to store the computers hardware and software information.
CREATE TABLE Framing.ComputerInfo (
	ComputerId int NOT NULL FOREIGN KEY REFERENCES Framing.Computers(Id),
	OSName varchar(20) NOT NULL,
	OSVersion varchar(20) NOT NULL,
	Username varchar(20) NOT NULL,
	Manufacturer varchar(20) NOT NULL,
	Model varchar(20) NOT NULL,
	BIOSVersion varchar(20) NOT NULL,
	BIOSNumber varchar(20) NOT NULL,
	MACAddress varchar(20) NOT NULL,
	RAM int NOT NULL,
);
GO

--Insert error records, make sure these have a computer id of 1.
INSERT INTO Framing.Computers (ExternalIP, MachineName) VALUES ('Not Found', 'Not Found');
INSERT INTO Framing.ComputerInfo VALUES (1, 'Not Found','Not Found','Not Found','Not Found','Not Found','Not Found','Not Found','Not Found', 0)
GO

--Creates a table to store tickets, stores a foreign key to the computer that the ticket was created on.
CREATE TABLE Framing.Ticket (
	Id int NOT NULL IDENTITY,
	ComputerId int NOT NULL FOREIGN KEY REFERENCES Framing.Computers(Id),
	TicketName varchar(50) NOT NULL,
	TicketDescription varchar(200) NOT NULL,
	TicketSeverity int NOT NULL,
);
GO

--Table to store FAQ questions
CREATE TABLE Framing.FAQ (
	Id int NOT NULL IDENTITY PRIMARY KEY,
	Question VARCHAR(200) NOT NULL UNIQUE,
	Description VARCHAR(200) NOT NULL,
	QuestionCount int NOT NULL DEFAULT 1
);
GO

--Table to store FAQ Answers
CREATE TABLE Framing.FAQAnswers (
	FAQId int NOT NULL FOREIGN KEY REFERENCES Framing.FAQ(Id),
	Answer VARCHAR(200) NOT NULL
);
GO

/*===== Creating Database Triggers =====*/
CREATE TRIGGER Computers_After_Insert ON Framing.Computers FOR INSERT
AS
	BEGIN
		SET NOCOUNT ON
		UPDATE 
			Framing.Computers
		SET
			 CreatedDate = GETUTCDATE()
			,CreatedBy = suser_sname()
		FROM
			Framing.Computers
			JOIN inserted ON Computers.Id = inserted.Id
	END
GO

CREATE TRIGGER Computers_After_Update ON Framing.Computers FOR UPDATE
AS
	BEGIN
		SET NOCOUNT ON
		UPDATE 
			Framing.Computers
		SET
			 ModifiedDate = GETUTCDATE()
			,ModifiedBy = suser_sname()
		FROM
			Framing.Computers
			JOIN inserted ON Computers.Id = inserted.Id
	END
GO

/*===== Creating Stored Procedures =====*/

/*Ensure an entry for a computer exists, if not, create an entry. Returns the ID of the computer*/
CREATE PROCEDURE EnsureComputerExists @MachineName VARCHAR(20), @ExternalIP VARCHAR(20) AS
BEGIN
	IF EXISTS(SELECT 1 FROM Framing.Computers WHERE Computers.MachineName = @MachineName)
		BEGIN
			IF EXISTS(SELECT 1 FROM Framing.Computers WHERE Computers.MachineName = @MachineName AND Computers.ExternalIP = @ExternalIP)
				BEGIN
					SELECT Id, 'Computer Exists' AS INFO FROM Framing.Computers WHERE Computers.MachineName = @MachineName AND Computers.ExternalIP = @ExternalIP;
				END
			ELSE
				BEGIN
					UPDATE Framing.Computers
						SET Computers.ExternalIP = @ExternalIP
					WHERE
						Computers.MachineName = @MachineName;
					SELECT Id, 'Updated IP' AS INFO FROM Framing.Computers WHERE Computers.MachineName = @MachineName;
				END
		END
	ELSE
		BEGIN
			CREATE TABLE #ComputerTemp(Id int);
			INSERT INTO Framing.Computers (ExternalIP, MachineName) OUTPUT INSERTED.Id INTO #ComputerTemp VALUES (@ExternalIP, @MachineName);
			SELECT *, 'Created Entry' AS INFO FROM #ComputerTemp;
			DROP TABLE #ComputerTemp;
		END
END
GO

--This procedure creates tickets in the database. Returns the ID of the created ticket.
CREATE PROCEDURE CreateTicket 
@ComputerId int, @TicketName varchar(50), @TicketDescription varchar(200), @TicketSeverity int AS
BEGIN
	CREATE TABLE #TicketTemp(Id int);
	INSERT INTO Framing.Ticket (ComputerId, TicketName, TicketDescription, TicketSeverity) OUTPUT INSERTED.Id INTO #TicketTemp VALUES (@ComputerId, @TicketName, @TicketDescription, @TicketSeverity);
	SELECT Id, 'Created Ticket' AS Info FROM #TicketTemp;
	DROP TABLE #TicketTemp;
END

GO

--Creates a Computer Info entry for a computer that is already in the database. It will update the database if there is already an entry.
CREATE PROCEDURE InsertMachineInfo
@ComputerId int, @OSName VARCHAR(20), @OSVersion VARCHAR(20), @Username VARCHAR(20), @Manufacturer VARCHAR(20),
@Model VARCHAR(20), @BIOSVersion VARCHAR(20), @BIOSNumber VARCHAR(20), @MACAddress VARCHAR(20), @RAM VARCHAR(20) AS
BEGIN
	IF EXISTS (SELECT 1 FROM Framing.ComputerInfo WHERE ComputerId = @ComputerId)
		BEGIN
			UPDATE Framing.ComputerInfo
			SET	 OSName			= @OSName
				,OSVersion		= @OSVersion
				,Username		= @Username
				,Manufacturer	= @Manufacturer
				,Model			= @Model
				,BIOSVersion	= @BIOSVersion
				,BIOSNumber		= @BIOSNumber
				,MACAddress		= @MACAddress
				,RAM			= @RAM
			WHERE
				ComputerId = @ComputerId
			SELECT 'Updated Entry' AS Info
		END
	ELSE
		BEGIN
			INSERT INTO Framing.ComputerInfo (ComputerId, OSName, OSVersion, Username, Manufacturer, Model, BIOSVersion, BIOSNumber, MACAddress, RAM)
			VALUES (@ComputerId, @OSName, @OSVersion, @Username, @Manufacturer, @Model, @BIOSVersion, @BIOSNumber, @MACAddress, @RAM)
			SELECT 'Created Entry' AS Info
		END
END
GO

--Returns the computer id of the given IP, if there are multiple it will return the most recently updated one.
CREATE PROCEDURE GetComputerByIP
@ExternalIP VARCHAR(20) AS 
BEGIN
	SELECT TOP 1 Id FROM Framing.Computers WHERE ExternalIP = @ExternalIP ORDER BY ModifiedDate DESC 	
END

GO

--Returns machine info given a machine name
CREATE PROCEDURE GetMachineInfoByMachineName @MachineName VARCHAR(20)
AS
BEGIN
	SELECT 
		ComputerInfo.*
	FROM 
		Framing.ComputerInfo
		LEFT JOIN Framing.Computers ON Computers.Id = ComputerInfo.ComputerId
	WHERE
		Computers.MachineName = @MachineName
END

GO

--Returns the entirity of the FAQ table, and answers, in a format that the WCF application can parse.
CREATE PROCEDURE GetFAQ
AS
BEGIN
	SELECT
		Id
		,Question
		,Description
		,QuestionCount
		,ISNULL(STUFF((
			SELECT
				'|' + Answer
			FROM
				Framing.FAQAnswers
			WHERE
				FAQAnswers.FAQId = OuterFAQ.Id
			FOR XML PATH('')
		), 1, 1, ''), '') AS Answers
	FROM
		Framing.FAQ AS OuterFAQ
END
GO

--Creates an FAQ question
CREATE PROCEDURE CreateQuestion
@Question VARCHAR(200)
,@Description VARCHAR(200)
AS
BEGIN
	IF(@Question IN (SELECT Question FROM Framing.FAQ))
	BEGIN
		SELECT 0 AS [Id];
	END
	ELSE
	BEGIN
		CREATE TABLE #FAQTemp(Id int);
		INSERT INTO Framing.FAQ (Question, Description, QuestionCount) OUTPUT INSERTED.Id INTO #FAQTemp VALUES (@Question, @Description, 1);
		SELECT Id FROM #FAQTemp;
		DROP TABLE #FAQTemp;
	END
END
GO

--Creats an FAQ answer
CREATE PROCEDURE CreateAnswer
@QuestionId int
,@Answer VARCHAR(200)
AS
BEGIN
	IF (@Answer IN (SELECT Answer FROM Framing.FAQAnswers WHERE FAQAnswers.FAQId = @QuestionId))
	BEGIN
		SELECT 'Duplicate Answer' AS [Output];
	END
	ELSE
	BEGIN
		INSERT INTO Framing.FAQAnswers (FAQId, Answer) VALUES (@QuestionId, @Answer);
		SELECT 'Answer Insterted' AS [Output];
	END
END
GO

--Gets Ticket Info with its corresponding computer info.
CREATE PROCEDURE GetTicket 
@TicketId int
AS
BEGIN
	SELECT
		TicketName
		,TicketDescription
		,TicketSeverity
		,ExternalIP
		,MachineName
		,CreatedDate
		,OSName
		,OSVersion
		,Username
		,Manufacturer
		,Model
		,BIOSVersion
		,MACAddress
		,RAM
	FROM
		Framing.Ticket
		LEFT JOIN Framing.Computers ON Ticket.ComputerId = Computers.Id
		LEFT JOIN Framing.ComputerInfo ON Computers.Id = ComputerInfo.ComputerId
	WHERE
		Ticket.Id = @TicketId
END
GO
/*===== Create a login for the WCF Service =====*/
--Make sure to change this password and update the connections string in the WCF Service with the new password
--This user needs permission to read, write, and execute stored procedures.
CREATE LOGIN Kutak_Rock_WCF WITH Password = '';
CREATE USER Kutak_Rock_WCF FROM LOGIN Kutak_Rock_WCF;

EXEC sp_addrolemember N'db_datareader', N'Kutak_Rock_WCF'  
EXEC sp_addrolemember N'db_datawriter', N'Kutak_Rock_WCF' 
GRANT EXECUTE TO Kutak_Rock_WCF
GO
/*==============================================*/
