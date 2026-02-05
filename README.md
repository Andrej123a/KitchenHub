KitchenHub

KitchenHub is a web-based restaurant management application developed using ASP.NET Core (.NET 8). The project is designed to support core restaurant operations such as managing menu categories, menu items, and customer orders through a structured and user-friendly admin interface.

The application includes a central dashboard that provides an overview of the restaurant’s activity, including total orders, revenue, menu availability, and order statuses. This allows quick insight into overall performance and daily operations.

KitchenHub supports full CRUD functionality for both categories and menu items, with additional features such as image previews, availability control, and category-based navigation. When adding new menu items, the system can automatically prefill data using recommendations from an external food API.

Order management is implemented with a clear status workflow, allowing orders to progress through predefined stages from creation to delivery. Each order includes a detailed view with item breakdowns and a visual progress indicator.

The project follows a layered architecture, separating the web layer, business logic, and data access for better maintainability and scalability. Data is stored in a SQL Server database using Entity Framework Core.

An external REST API (TheMealDB) is integrated to enhance functionality by providing real-world food data for recommendations and auto-filling menu item information.

KitchenHub is developed as an academic project and demonstrates practical usage of modern .NET technologies, MVC architecture, database integration, and external API consumption.
