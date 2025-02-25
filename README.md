### **GrubPix**

GrubPix is a web-based visual menu platform designed to revolutionize the way restaurants present their menus. Instead of relying on traditional text-based menus, GrubPix allows restaurants to showcase their dishes with vibrant, mouth-watering images that entice customers and enhance their dining experience.

---

### **🚀 Features**

- **Visual Menus:** Display food items with high-quality images.
- **Restaurant & Menu Management:** Easily add, update, or remove restaurants and their menus.
- **QR Code Integration:** Allow customers to scan and view menus on their phones.
- **Admin Dashboard:** Manage restaurants, menus, and items with ease.
- **Future Enhancements:** AI-powered recommendations, AR food previews, and online ordering integration.

---

### **🛠 Tech Stack**

- **Frontend:** Blazor WebAssembly (Planned)
- **Backend:** .NET Core API
- **Database:** PostgreSQL
- **Architecture:** Clean Architecture with MediatR, AutoMapper, and Dependency Injection

---

### **📁 Project Structure**

- `GrubPix.API` - The main API project
- `GrubPix.Application` - Core business logic
- `GrubPix.Domain` - Domain models and entities
- `GrubPix.Infrastructure` - Data access and external services

---

## **🚀 Getting Started**

### **1️⃣ Clone the repository**

```bash
git clone git@github.com:GrubPix/GrubPix.git
cd GrubPix
```

### **2️⃣ Setup the database**

- Ensure **PostgreSQL** is installed and running.
- Update the connection string in `appsettings.json`.

### **3️⃣ Run the application**

```bash
dotnet build
dotnet run --project GrubPix.API
```

---

## **🐳 Running with Docker**

You can also run the application using Docker.

### **1️⃣ Build and run the Docker containers**

```bash
docker-compose up --build
```

This command will:

- Start the **GrubPix API**
- Start a **PostgreSQL** database instance
- Connect them together via Docker Compose

### **2️⃣ Stop the containers**

To stop the running containers, use:

```bash
docker-compose down
```

---

## **🔗 Access Swagger UI**

Once the API is running, visit:

[http://localhost:5068/index.html](http://localhost:5068/index.html)

to test the API endpoints.

---

## **📝 API Endpoints**

### **🔑 Auth**

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Authenticate and log in
- `POST /api/auth/verify` - Verify authentication status

### **👤 Customer**

- `GET /api/customers/profile` - Get customer profile
- `PUT /api/customers/profile` - Update customer profile
- `GET /api/customers/favorites` - Get customer favorite menu items
- `POST /api/customers/favorites/{menuItemId}` - Add a menu item to favorites
- `DELETE /api/customers/favorites/{menuItemId}` - Remove a menu item from favorites

### **🍽️ Menu**

- `GET /api/menus` - Retrieve all menus
- `POST /api/menus` - Create a new menu
- `GET /api/menus/{id}` - Retrieve a specific menu by ID
- `PUT /api/menus/{id}` - Update a menu
- `DELETE /api/menus/{id}` - Delete a menu

### **🥘 MenuItem**

- `GET /api/MenuItem` - Retrieve all menu items
- `POST /api/MenuItem` - Create a new menu item
- `GET /api/MenuItem/{id}` - Retrieve a specific menu item by ID
- `PUT /api/MenuItem/{id}` - Update a menu item
- `DELETE /api/MenuItem/{id}` - Delete a menu item
- `DELETE /api/MenuItem/{menuItemId}/images/{imageId}` - Delete a menu item image

### **🏬 Restaurant**

- `GET /api/restaurants` - Retrieve all restaurants
- `POST /api/restaurants` - Create a new restaurant
- `GET /api/restaurants/{id}` - Retrieve a specific restaurant by ID
- `PUT /api/restaurants/{id}` - Update restaurant details
- `DELETE /api/restaurants/{id}` - Delete a restaurant

### **👤 User**

- `PUT /api/users/{id}` - Update user information
- `DELETE /api/users/{id}` - Delete a user

---

**GrubPix – Because we eat with our eyes first!** 🍽️👀
