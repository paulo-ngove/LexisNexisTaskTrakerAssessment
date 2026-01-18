# **TaskTracker API (Backend)**

A robust task management API built with .NET 10, Entity Framework Core, and InMemory Database. Features full CRUD operations, validation, sorting, search, and RFC 7807 ProblemDetails error handling.

## **Prerequisites**

Before you begin, ensure you have the following installed:
- **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)** or later
- **Docker & Docker Compose** (optional if using Docker - for containerized setup)
- **Git** for version control

## **Getting Started**

### **1. Clone and Configure**

```bash
git clone <repository-url>
cd TaskTrackerAPI
```

### **2. Run Locally (Development)**
```bash
# Restore dependencies
dotnet restore

# Run the application
dotnet run --urls=http://localhost:7155
```

The API will be available at `http://localhost:7155`
- **Swagger UI**: `http://localhost:7155/swagger`

## **API Endpoints**
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/tasks` | Get all tasks with optional `q` (search) and `sort` params |
| `GET` | `/api/tasks/{id}` | Get a specific task by ID |
| `POST` | `/api/tasks` | Create a new task |
| `PUT` | `/api/tasks/{id}` | Update an existing task |
| `DELETE` | `/api/tasks/{id}` | Delete a task |
| `PATCH` | `/api/tasks/{id}/complete` | Mark task as complete |
| `PATCH` | `/api/tasks/{id}/incomplete` | Mark task as incomplete |

## **Testing**

### **Run Unit Tests**
```bash
dotnet test
```

## **CI/CD Pipeline**
The project includes GitHub Actions workflows that:
1. **Run tests** on push/PR to main/develop branches
2. **Build Docker image** and push to GitHub Container Registry
3. **Security scan** with Trivy and OWASP Dependency Check
4. **Deploy automatically** to development on merge to `develop`
5. **Require approval** for production deployment from `main`

Trigger manually from Azure DevOps or on code push.

## **Project Structure**
```
TaskTrackerAPI/
├── Controllers/          # API endpoints
├── Domain/              # Models, DTOs, Enums
├── Infrastructure/      # DbContext and configurations
├── Program.cs           # Startup and DI configuration
└── TaskTrackerAPI.csproj
```
---

# **TaskTracker SPA (Frontend)**

A modern task management single-page application built with React 19, TypeScript, Vite, and Tailwind CSS. Features responsive design, real-time updates, and a polished UI.

## **Prerequisites**

- **Node.js 22.18.0** or later ([Download](https://nodejs.org/))
- **npm** or **yarn** package manager
- **Backend API** running (see Backend README)

## **Getting Started**

### **1. Clone and Install**
```bash
git clone <repository-url>
cd task-tracker-spa
```

Install dependencies using your preferred package manager:

**Using npm:**
```bash
npm install
```

**Using yarn:**
```bash
yarn install
```

### **2. Configure Environment**
Create a `.env` file in the root:
```env
VITE_API_BASE_URL=http://localhost:7511
```

### **3. Run Development Server**
**Using npm:**
```bash
npm run dev
```

**Using yarn:**
```bash
yarn dev
```

The application will open at `http://localhost:3000`

## **Available Scripts**

### **Development**
| Command | npm | yarn | Description |
|---------|-----|------|-------------|
| Start dev server | `npm run dev` | `yarn dev` | Starts Vite dev server with HMR |
| Build for production | `npm run build` | `yarn build` | Creates optimized production build |
| Preview production build | `npm run preview` | `yarn preview` | Locally preview production build |
| Lint code | `npm run lint` | `yarn lint` | Run ESLint |
| Type check | `npm run type-check` | `yarn type-check` | Run TypeScript compiler check |

### **Testing**
| Command | npm | yarn | Description |
|---------|-----|------|-------------|
| Run unit tests | `npm test` | `yarn test` | Run Vitest tests |
| Run tests with UI | `npm run test:ui` | `yarn test:ui` | Open Vitest UI |
| Run tests with coverage | `npm run test:coverage` | `yarn test:coverage` | Generate test coverage report |


## **Project Structure**
```
src/
├── api/              # API client and types
├── components/       # Reusable React components
│   ├── TaskList.tsx
│   ├── TaskForm.tsx
│   └── LoadingSpinner.tsx
├── pages/           # Page components
│   ├── TaskListView.tsx
│   └── TaskFormPage.tsx
├── utils/           # Utility functions
│   ├── validation.ts
│   └── formatters.ts
├── App.tsx          # Main application component
└── main.tsx         # Application entry point
```

## **Key Features**
-  **Task Search & Filtering**: Real-time search across titles and descriptions
-  **Responsive Design**: Mobile-first approach with Tailwind CSS
-  **Fast Performance**: Built with Vite for instant hot module replacement
-  **Type Safety**: Full TypeScript support throughout
-  **Modern UI**: Clean interface with Tailwind CSS and Lucide React icons
-  **Real-time Updates**: Interactive task completion toggling
-  **Form Validation**: Client-side validation mirroring backend rules

## **Technologies Used**
- **React 19** - UI library
- **TypeScript** - Type safety
- **Vite** - Build tool and dev server
- **Tailwind CSS** - Styling framework
- **React Router DOM** - Client-side routing
- **Axios** - HTTP client
- **Lucide React** - Icon library
- **date-fns** - Date formatting
- **Vitest** - Testing framework

## **Connecting to Backend**
By default, the app connects to `http://localhost:7155`. To change this:
1. Update the `VITE_API_BASE_URL` in `.env`
2. Restart the development server
3. The proxy in `vite.config.ts` will forward API requests

## **Common Issues & Solutions**

### **Backend Connection Failed**
```bash
# Update .env file with correct URL
VITE_API_BASE_URL=http://your-backend-url:port
```

### **Node Version Mismatch**
Ensure you're using Node 22.18.0:
```bash
node --version  # Should show v22.18.0

# Using nvm (recommended)
nvm install 22.18.0
nvm use 22.18.0
```

### **Port Already in Use**
Change the port in `package.json`:
```json
"scripts": {
  "dev": "vite --port 3001"
}
```

---

## **Development Workflow**

1. **Start the backend** (Docker or local)
2. **Start the frontend**: `npm run dev` or `yarn dev`
3. **Make changes** - Vite will hot reload automatically
4. **Run tests** before committing: `npm test` or `yarn test`
5. **Build for production**: `npm run build` or `yarn build`
