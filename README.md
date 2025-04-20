# 🛍️ VStore - E-commerce Platform by anhvu04

<div align="center">

[![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)

A modern e-commerce platform built with .NET 8, implementing Clean Architecture, CQRS pattern, and cutting-edge web technologies.

</div>

## 📋 Table of Contents
- [🎯 Overview](#overview)
- [🏗️ Architecture](#architecture)
- [💻 Tech Stack](#tech-stack)
- [⭐ Features](#features)
- [🚀 Getting Started](#getting-started)
- [📚 API Documentation](#api-documentation)
- [📦 Deployment](#deployment)

## 🎯 Overview

VStore is a modern e-commerce solution that provides a robust API for managing products, orders, payments, and user interactions. It supports real-time communications, multiple payment gateways, and integrated shipping services.

## 🏗️ Architecture

The solution follows Clean Architecture principles with the following structure:

```plaintext
VStore/
├── 🎯 VStore.API/           # API endpoints, configurations, and middleware
├── 💼 VStore.Application/   # Application logic, CQRS handlers, validations
├── 🏢 VStore.Domain/        # Domain entities, business rules
├── 🔧 VStore.Infrastructure/# External services, implementations
└── 💾 VStore.Persistence/   # Database context, configurations
```

## 💻 Tech Stack

### 🔧 Core Framework
- **.NET 8.0**
- **ASP.NET Core Web API**
- **Entity Framework Core**

### 🔐 Authentication & Security
- **JWT Bearer Authentication**
- **BCrypt** - Password hashing
- **Role-based Authorization**

### 💾 Database & Caching
- **SQL Server** - Primary database
- **Redis** - Distributed caching
  - 🛒 Shopping cart
  - 📦 API response caching
  - 🔑 Session management

### 📨 Message Queue & Background Jobs
- **RabbitMQ** - Message broker for:
  - 📧 Email notifications
  - 💳 Payment processing
  - 📦 Order updates
- **Quartz.NET** - Scheduled tasks

### 🔄 Real-time Communication
- **SignalR**
  - 💬 Real-time chat
  - 👥 Presence tracking
  - 🔔 Live notifications

### 🌐 External Services
- **PayOS** - Payment processing
- **VNPay** - Vietnamese payment gateway
- **GHN Express** - Delivery service
- **Cloudinary** - Cloud storage
- **SMTP** - Email service

### 📊 Logging & Monitoring
- **Serilog**
  - 🖥️ Console logging
  - 📁 File logging
  - 📈 Structured logging

## ⭐ Features

### 📦 Product Management
- ✨ CRUD operations
- 🗂️ Category management
- 🖼️ Image handling
- 📊 Inventory tracking

### 👥 User Management
- 🔐 Authentication
- 👮 Role-based authorization
- 👤 Profile management
- 📝 Activity tracking

### 🛒 Order System
- 🛍️ Shopping cart (Redis)
- 📦 Order processing
- 💳 Payment integration
- 🚚 Shipping tracking

### 💰 Payment Integration
- 💳 Multiple payment gateways
- 📊 Payment status tracking
- ↩️ Refund handling
- 📜 Transaction history

### ⚡ Real-time Features
- 💬 Live chat
- 🔔 Order notifications
- 👥 User presence
- 📊 Status updates

## 🚀 Getting Started

### 📋 Prerequisites
- ✅ .NET 8.0 SDK
- 💾 SQL Server (can use docker)
- 📦 Redis Server
- 🐰 RabbitMQ Server (can use docker)
- 💻 IDE (Visual Studio 2022 or JetBrains Rider)

### ⚙️ Installation

1. Clone the repository:
```bash
git clone https://github.com/anhvu04/VStore_Project.git
```

2. Configure services in `appsettings.json`:
```
💬 Connect with me to get a sample config: https://www.facebook.com/anhvu21.08.04
```

3. Apply database migrations:
```bash
dotnet ef database update --project VStore.Persistence --startup-project VStore.API
```

4. Run the application:
```bash
dotnet run --project VStore.API
```

### 🌐 Access Points
- 🔒 HTTPS: https://localhost:5000
- 📚 Swagger UI: https://localhost:5000/swagger

## 📦 Deployment

### 🐳 Docker Support
```bash
docker build -t vstore-api .
docker run -p 5000:80 vstore-api
```

### ⚙️ ENV File
```
📝 See github file: .env.example
```

## 📚 API Documentation
### 
- 📖 Docs: https://localhost:5000/docs/api


## 📄 License

[panhvuu04] © [Phan Anh Vu]

---
<div align="center">

### 🌟 Connect with me

[![Facebook](https://img.shields.io/badge/Facebook-1877F2?style=for-the-badge&logo=facebook&logoColor=white)](https://www.facebook.com/anhvu21.08.04)
[![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/anhvu04)

</div>