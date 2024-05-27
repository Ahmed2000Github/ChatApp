 <img src="https://github.com/Ahmed2000Github/ChatApp/blob/master/ChatAppShared/wwwroot/favicon.png?raw=true" width="50" title="chat app"> **SignalR Real-Time Chat Application**
 

This project is a simple real-time chat application built with .NET and SignalR. Users can log in, create conversations with others, share insights, and exchange files.

## Features

- **User Authentication**: Secure login functionality.
- **Real-Time Communication**: Instant messaging with SignalR.
- **Conversations**: Create and join conversations with other users.
- **File Sharing**: Share files seamlessly within the chat.

## Video Demo

 Click the image to see the demonstration video

 
[<img src="https://github.com/Ahmed2000Github/ChatApp/assets/93035933/fdd90d17-b43b-4114-8ca3-7147271b0ed1" width="300" title="chat app video"> ](https://raw.githubusercontent.com/Ahmed2000Github/ChatApp/master/ChatApp-demo.mp4)


## Technologies Used

- **.NET**
- **SignalR**
- **ASP.NET Core**
- **Blazor**
- **MYSQL**

## Getting Started

1. **Clone the Repository**:
   
   ```sh
   git clone https://github.com/username/repository.git
   cd repository
   
3. **DataBase setup**:
   - In your MySQL Database create new database.
   - In the poject appsettings.json set your own "ConnectionString" to your database.
   - In your package manager console and under the ChatAppInfrastructure folder run the two following commands to migrate data:
     
      ```sh
       dotnet ef migrations add initialeCreate -s ..\ChatAppServer\ChatAppServer.csproj
       dotnet ef database update -s ..\ChatAppServer\ChatAppServer.csproj
      
4. **Build and Run**:
   
   ```sh
    dotnet build
    dotnet run
   
6. **Access the Application**:

   After running your add you can access to the app by the following:
   
 - Backend: Open your browser and navigate to https://localhost:7134.
 - Frontend: Open your browser and navigate to https://localhost:7075.

Contributing
Contributions are welcome! Please fork the repository and submit a pull request.

License
This project is licensed under the MIT License.
