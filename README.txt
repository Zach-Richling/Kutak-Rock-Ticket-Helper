This Solution contains the two main components of the Kutak Rock Ticketing Backend: The WCF Service and the Client Side Windows Service.

The WCF service is a webapi that will be hosted on azure that will take requests from the front end web application and the backend client side windows service.
Its main use is to relay data from both of those parts of the application to the database.

The client side helper is resposible for obtaining machine information about the machine it is installed on and relaying that information to the data base
to be submitted with a ticket from the same computer. 