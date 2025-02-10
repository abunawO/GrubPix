# GrubPix

GrubPix is a web-based visual menu platform designed to revolutionize the way restaurants present their menus. Instead of relying on traditional text-based menus, GrubPix allows restaurants to showcase their dishes with vibrant, mouth-watering images that entice customers and enhance their dining experience.

## 🚀 Features
- **Visual Menus:** Display food items with high-quality images.
- **Restaurant & Menu Management:** Easily add, update, or remove restaurants and their menus.
- **QR Code Integration:** Allow customers to scan and view menus on their devices.
- **Admin Dashboard:** Manage restaurants, menus, and items with ease.
- **Future Enhancements:** AI-powered recommendations, AR food previews, and online ordering integration.

## 🧰 Tech Stack
- **Frontend:** Blazor WebAssembly
- **Backend:** .NET Core API
- **Database:** PostgreSQL
- **Architecture:** Clean Architecture with MediatR, AutoMapper, and Dependency Injection

## 📦 Project Structure
- `GrubPix.API` - The main API project
- `GrubPix.Application` - Core business logic
- `GrubPix.Domain` - Domain models and entities
- `GrubPix.Infrastructure` - Data access and external services

## ⚡ Getting Started
1. **Clone the repository:**
   ```bash
   git clone git@github.com:abunawO/GrubPix.git
   cd GrubPix
   ```

2. **Setup the database:**
   - Ensure PostgreSQL is installed and running.
   - Update the connection string in `appsettings.json`.

3. **Run the application:**
   ```bash
   dotnet build
   dotnet run --project GrubPix.API
   ```

4. **Access Swagger UI:**
   Visit `http://localhost:5068/index.html` to test the API endpoints.

## 📋 API Endpoints
- `GET /api/restaurants` - Retrieve all restaurants
- `GET /api/restaurants/{id}` - Retrieve a specific restaurant by ID
- `POST /api/restaurants` - Create a new restaurant
- `PUT /api/restaurants/{id}` - Update restaurant details
- `DELETE /api/restaurants/{id}` - Delete a restaurant

## 🤝 Contributing
Contributions are welcome! Please fork the repository, create a new branch, and submit a pull request with your changes.

## 📜 License
This project is licensed under the MIT License.

## 📧 Contact
For any inquiries or suggestions:
- **Developer:** abunawO
- **Email:** abunawose@gmail.com

---
GrubPix - Because we eat with our eyes first! 🍽️

