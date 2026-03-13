# ✈️ Voya - High-Performance Hotel Booking System

<p align="center">
  <img src="https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=.net&logoColor=white" />
  <img src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white" />
  <img src="https://img.shields.io/badge/Stripe-008CD1?style=for-the-badge&logo=stripe&logoColor=white" />
  <img src="https://img.shields.io/badge/JWT-black?style=for-the-badge&logo=json-web-tokens&logoColor=white" />
</p>

**Voya** is an enterprise-grade Hotel Booking API built with **ASP.NET Core 8**. This project moves beyond simple CRUD operations by implementing a **Distributed ID Generation** system, custom security handshakes, and a professional **Stripe** payment integration.

--- 

## 🚀 Key Technical Highlights

* **Custom Snowflake IDs**: Designed for high scalability. Every ID (Users, Bookings, Transactions) is a 64-bit unique `long` generated manually to prevent primary key collisions in distributed environments.
* **Stripe Payment Integration**: A full lifecycle implementation featuring session creation, transaction logging, and booking state management.
* **Security-First Architecture**:
    * **Custom Security Middleware**: Validates requests via a secure secret-key handshake.
    * **JWT & Refresh Tokens**: Secure authentication with Role-Based Access Control (RBAC).
* **Advanced EF Core**: 
    * **Fluent API**: Precise database schema control.
    * **Global Query Filters**: Automated handling of **Soft Delete** logic.

---

## 🛠️ Tech Stack

| Category | Technology |
| :--- | :--- |
| **Framework** | ASP.NET Core 8 (Web API) |
| **Database** | SQL Server |
| **ORM** | Entity Framework Core |
| **Identity** | Custom 64-bit Snowflake ID Generator |
| **Payments** | Stripe API |
| **Security** | JWT + Custom Middleware Handshake |

---

## 📦 Installation & Setup

### 📥 Clone the Repository

[![Git Clone](https://img.shields.io/badge/GIT-Clone_Project-blue?style=for-the-badge&logo=github)](https://github.com/kurtbey1/Voya.git)

```bash
git clone [https://github.com/kurtbey1/Voya.git](https://github.com/kurtbey1/Voya.git)
cd Voya
```
⚙️ Configuration
Update appsettings.json with your specific keys:
```bash
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=VoyaDB;Trusted_Connection=True;"
  },
  "Stripe": {
    "SecretKey": "sk_test_YourKey"
  },
  "Security": {
    "SecretKey": "Your_Super_Secret_Handshake_Key"
  }
}
```
🚀 Database Migration
```bash
dotnet ef database update
```

🔒 Security HandshakeTo protect the API, every request must include:HeaderValueAuthorizationBearer <Your_JWT_Token>X-Voya-Secret<Your_Configured_Secret_Key>

🛡️ Technical Challenges Overcome
The ID Collision Challenge: Standard SQL Identities can be predictable and difficult to merge across databases. I implemented a custom IdGenerator producing unique 64-bit IDs. This ensures every entity has a unique identity across a distributed system, requiring deep integration with EF Core's ValueGeneratedNever() configuration.

<p align="center">Developed with 🔥 by <strong>Abedalrhman</strong></p>
