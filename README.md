# Guider.pro - SEO-Friendly Blazor Business Catalog

Guider.pro is a modern, high-performance web application built with .NET 8 and Blazor, designed to serve as an SEO-friendly catalog for local businesses.

This project leverages Blazor's Static Server-Side Rendering (SSR) and Stream Rendering to ensure content is fully indexable by search engines, providing excellent SEO performance right out of the box. The frontend is interactive where needed (e.g., filters and theme toggling) but defaults to static rendering for speed and SEO.

## ✨ Key Features

- **SEO-First Architecture**: Built using Blazor's SSR and Stream Rendering. All business data is rendered on the server, making it perfectly visible to search engine crawlers.
- **Dynamic Detail Pages**: Unique, server-rendered pages for each business (`/place/{id}`) with schema.org metadata (LocalBusiness) embedded for Rich Results.
- **Interactive Filtering**: A hybrid-mode component (`PlacesFilter.razor`) allows users to filter the business list interactively (using Blazor Server/WASM) without a full page reload.
- **Pagination**: Efficiently handles large datasets with server-side pagination, ensuring fast initial loads.
- **Dark/Light Theme**: A persistent, cookie-based theme toggle (`NavBar.razor`) that works across both static (SSR) and interactive pages, preventing any "flickering" on page load.
- **Responsive Design**: Mobile-first CSS ensures the catalog is usable on all devices, with a "hamburger" menu for navigation.
- **Shared Logic**: The `GuiderBlazor.Shared` project ensures that models and services are shared between the server (SSR) and client (interactive) projects, reducing code duplication.

## 🚀 Tech Stack

- **.NET 8**
- **ASP.NET Core**
- **Blazor Web App** (SSR, Stream Rendering, and Interactive Server modes)
- **C#**
- **CSS** (with CSS Variables for easy theming)
- **JavaScript (JS Interop)**: Used sparingly for theme persistence (LocalStorage/Cookies).

## 🔧 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A C# IDE (like [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/))

### Running the Application

1.  **Clone the repository:**

    ```bash
    git clone [URL-вашего-репозитория]
    cd [имя-папки-проекта]
    ```

2.  **Restore dependencies:**

    ```bash
    dotnet restore
    ```

3.  **Run the project:**
    The application runs from the main server project (`GuiderBlazor.csproj`).
    ```bash
    dotnet run --project GuiderBlazor
    ```
    The application will be available at `http://localhost:XXXX` and `https://localhost:XXXX`.

### Configuration

The application connects to a backend API to fetch business listings. This is configured in `Program.cs`:

```csharp
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("[https://api.guider.pro/](https://api.guider.pro/)")
});
```
