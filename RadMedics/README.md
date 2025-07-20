# RadMedics Learning Management System

A comprehensive Learning Management System (LMS) built with ASP.NET Core 8, MySQL, and Entity Framework Core. This system provides separate interfaces for administrators and students with full course management, forum communication, and calendar functionality.

## 🚀 Features

### Admin Dashboard
- **User Management**: View, search, and manage all users
- **Course/Module Management**: Create, edit, and delete courses with overlay modals
- **Assessment Management**: Manage assessments with toggle status controls
- **Statistics**: Real-time dashboard with user counts and module statistics
- **Settings**: Password change and logout functionality

### Student Interface
- **Dashboard**: Personalized home with course progress and upcoming events
- **Profile Management**: Edit personal information with modal overlays
- **Course Enrollment**: View enrolled courses and progress
- **Calendar System**: Create, edit, and manage events with drag-and-drop functionality
- **Forum/Email System**: Gmail-like communication system with inbox, drafts, and sent mail
- **Settings**: Password change and logout functionality

### Technical Features
- **Authentication & Authorization**: ASP.NET Core Identity with role-based access
- **Responsive Design**: Mobile-friendly interface
- **Real-time Updates**: AJAX-powered dynamic content
- **Database Management**: MySQL with Entity Framework Core migrations
- **Modern UI/UX**: Clean, professional interface with modal overlays

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8 MVC
- **Database**: MySQL 8.0
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap
- **Development**: Visual Studio 2022 / VS Code

## 📋 Prerequisites

Before running this application, make sure you have:

- **.NET 8 SDK** installed
- **MySQL 8.0** server running
- **Visual Studio 2022** or **VS Code** with C# extension

## 🔧 Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/YOUR_USERNAME/RadMedics.git
cd RadMedics
```

### 2. Database Setup
1. **Create MySQL Database**:
   ```sql
   CREATE DATABASE radmedics_db;
   ```

2. **Update Connection String**:
   - Open `appsettings.json`
   - Update the `DefaultConnection` string with your MySQL credentials:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=radmedics_db;Uid=your_username;Pwd=your_password;"
     }
   }
   ```

3. **Run Database Migrations**:
   ```bash
   cd RadMedics
   dotnet ef database update
   ```

### 3. Build and Run
```bash
dotnet build
dotnet run
```

### 4. Access the Application
- **Application URL**: `https://localhost:5001` or `http://localhost:5000`
- **Admin Login**: `admin@example.com` / `AdminPassword123!`
- **Student Login**: `student@example.com` / `StudentPassword123!`

## 📁 Project Structure

```
RadMedics/
├── Controllers/          # MVC Controllers
│   ├── AdminController.cs
│   ├── StudentController.cs
│   ├── ForumController.cs
│   └── HomeController.cs
├── Models/              # Data Models
│   ├── ApplicationUser.cs
│   ├── Course.cs
│   ├── MailMessage.cs
│   └── ApplicationDbContext.cs
├── Views/               # Razor Views
│   ├── Admin/          # Admin interface views
│   ├── Student/        # Student interface views
│   ├── Forum/          # Forum/email system views
│   └── Shared/         # Layout files
├── Migrations/          # Entity Framework migrations
└── wwwroot/            # Static files (CSS, JS, images)
```

## 🔐 Default Users

The application comes with pre-seeded users:

### Admin User
- **Email**: `admin@example.com`
- **Password**: `AdminPassword123!`
- **Access**: Full admin dashboard with all management features

### Student User
- **Email**: `student@example.com`
- **Password**: `StudentPassword123!`
- **Access**: Student dashboard with course enrollment and forum access

## 🎯 Key Features Explained

### Admin Dashboard
- **Statistics Cards**: Real-time counts of users and modules
- **Module Management**: Add modules with overlay modals
- **User Management**: Complete user administration interface
- **Assessment Management**: Toggle-based assessment status control

### Student Features
- **Personal Dashboard**: Course progress and upcoming events
- **Calendar System**: Full event management with CRUD operations
- **Forum System**: Email-like communication with inbox, drafts, and sent mail
- **Profile Management**: Modal-based profile editing

## 🔄 Database Migrations

To create new migrations after model changes:
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

## 🚀 Deployment

### Local Development
```bash
dotnet run
```

### Production Deployment
1. **Build for Production**:
   ```bash
   dotnet publish -c Release
   ```

2. **Deploy to IIS or Azure**:
   - Copy published files to web server
   - Configure connection strings for production database
   - Set up SSL certificates

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

If you encounter any issues or have questions:

1. Check the [Issues](https://github.com/YOUR_USERNAME/RadMedics/issues) page
2. Create a new issue with detailed description
3. Include error messages and steps to reproduce

## 🔮 Future Enhancements

- [ ] Real-time notifications
- [ ] File upload functionality
- [ ] Advanced reporting and analytics
- [ ] Mobile app development
- [ ] Integration with external LMS APIs
- [ ] Multi-language support
- [ ] Advanced assessment types
- [ ] Video conferencing integration

---

**Built with ❤️ using ASP.NET Core 8** 