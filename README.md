# Gym Management Web Application

This repository contains an ASP.NET Core MVC project for managing a gym. It provides basic membership management, package registration, payment handling, a simple chatbot and notification features.

## Features

- **User Accounts**: Register, login and profile management with roles for Admin, Member, Staff and Trainer.
- **Membership Management**: CRUD operations for members and packages with expiration tracking.
- **Payments**: PayPal and VNPay integrations, unpaid package list and checkout pages.
- **Notifications**: Admins can send announcements to selected users, notifications are stored per user.
- **Feedback**: Members can send feedback and view their history, admins can view all feedback.
- **Chatbot**: Integrates with OpenAI to answer questions about available packages.

## Project Structure

- `gym/` – ASP.NET Core project source
  - `Controllers/` – MVC controllers (Account, Admin, Booking, etc.)
  - `Data/` – Entity Framework Core models and `GymContext`
  - `Services/` – PayPal, VNPay, Email and Chatbot services
  - `Templates/` – Email templates for OTP and payment confirmations
  - `Views/` – Razor views for the web UI
- `gym.sln` – Visual Studio solution file

## Requirements

- .NET SDK 9.0 or newer
- SQL Server instance (connection string configured in `appsettings.json`)
- Optional: Visual Studio 2022 or `dotnet` CLI

## Getting Started

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd courseproject-ltweb-gym
   ```
2. **Configure the database and services**
   - Copy `gym/appsettings.Development.json` to `gym/appsettings.json` and add:
     - `ConnectionStrings:DefaultConnection` for your SQL Server
     - `OpenAI` section with `ApiKey` and `BaseUrl`
     - `Paypal` and `VnPay` settings if you use those gateways
     - `EmailSettings` for the SMTP server
3. **Run the application**
   ```bash
   dotnet run --project gym
   ```
   The site will start on `http://localhost:5096` (see `launchSettings.json`).

Open the solution in Visual Studio if you prefer an IDE workflow.

## Note

The project references several NuGet packages (`DinkToPdf`, `EPPlus`, `itext7`, `PayPalCheckoutSdk` and others). Ensure these packages restore successfully when building the project.

## License

This project is provided for educational purposes. It does not include a specific license.
