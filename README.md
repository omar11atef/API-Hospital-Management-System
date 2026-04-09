# 🏥 Hospital Management System (HMS) API

## 📖 Overview
The Hospital Management System API is a robust, scalable backend solution designed to streamline the complex, day-to-day operations of a healthcare facility. Built with a highly normalized relational database architecture, this RESTful API manages everything from patient admissions and doctor scheduling to room availability and financial billing.

## Key Features & Services
This system is broken down into modular services to handle different hospital domains securely:

* **👥 User & Role Management:** Secure authentication and authorization using Role-Based Access Control (RBAC). Differentiates access levels between Admins, Doctors, Receptionists, and Patients.
* **🩺 Medical Records & Prescriptions:** Tracks patient visit history, diagnoses, and detailed medication dosages linked to specific appointments.
* **📅 Appointment Scheduling:** Endpoints to book, update, and track patient-doctor appointments while preventing scheduling conflicts.
* **🛏️ Facility Management:** Real-time tracking of hospital departments, room types, pricing, and patient check-in/check-out statuses to monitor bed occupancy.
* **💳 Billing & Invoicing:** Automatically correlates patient stays and medical services with generated invoices and line items.

## 🛠️ Tech Stack & Libraries
* **Framework:** ASP.NET Core Web API
* **Database ORM:** Entity Framework Core (EF Core)
* **Authentication:** JSON Web Tokens (JWT) for stateless, secure endpoint protection
* **Documentation:** Swagger / OpenAPI 

## 📊 Database Architecture (ERD)
The core of this system relies on a well-structured relational database ensuring data integrity. 

🔒 Security & Protection
Protected Endpoints: Most endpoints require a valid Bearer token. You must authenticate via the /api/Users/Login endpoint to receive a JWT.

Using Swagger with JWT: Click the "Authorize" button at the top of the Swagger UI and paste your token in the format: Bearer {your_token_here}.

---
🌐 Live Demo
You can explore the live, deployed version of the API and test its endpoints directly here:
[http://hosptial1system.runasp.net/swagger/index.html]
