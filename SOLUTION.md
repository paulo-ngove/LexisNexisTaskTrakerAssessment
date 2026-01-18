# TaskTracker Development: Debugging Issues & Solutions

## 1. Backend: AutoMapper Dependency Injection Failure
**Problem**: API crashed on startup with an `IMapper` service resolution error, despite correct-looking configuration.

**Diagnosis**: The issue was isolated to AutoMapper by creating a test controller without it, which worked. The root cause was the deprecated `AutoMapper.Extensions.Microsoft.DependencyInjection` package, which failed silently in .NET 8.

**Resolution**: AutoMapper was manually configured in the dependency injection container instead of using the broken extension method.

**Prevention**: Added constructor validation and a dedicated health endpoint to verify service registration.

## 2. Frontend: Hot Module Replacement Failure with Path Aliases
**Problem**: Changes to components using `@/` path aliases did not update in the browser, breaking the developer feedback loop.

**Diagnosis**: HMR worked for relative imports but failed for alias-based ones. The problem was Vite's dev server losing alias resolution context during hot updates.

**Resolution**: The Vite configuration was enhanced to force pre-bundling of alias-based imports, and import patterns were standardized.

**Prevention**: An ESLint rule was added to enforce consistent alias usage.

## 3. Backend: Database Connection Pool Exhaustion Under Load
**Problem**: During performance testing, the API started throwing `TimeoutException` errors after handling sustained concurrent requests.

**Diagnosis**: Logs showed a high number of concurrent connections. The default Entity Framework connection pooling strategy was insufficient. A connection leak was also identified in a background service that was not properly disposing of contexts.

**Resolution**:
1.  **Leak Fix**: Ensured all `DbContext` instances were wrapped in `using` statements or injected with the correct scoped lifetime.
2.  ‍**Pool Tuning**: The connection string was modified to increase the `Max Pool Size` and a retry policy with exponential backoff was added via `Polly`.

**Prevention**: Added monitoring for active database connections and integrated a chaos engineering test to simulate connection failures.

## 4. Frontend: Memory Leak in Task List Component
**Problem**: The application became progressively slower and unresponsive when switching between the task list and other views multiple times.

**Diagnosis**: Using the browser's Memory profiler revealed detached DOM elements and growing listener counts. The issue was in `TaskList.tsx`: event listeners on task action buttons were not being cleaned up, and `setInterval` in a custom hook was not cleared on unmount.

**Resolution**:
1.  Refactored event listeners to use React's synthetic event system.
2.  Added a cleanup function in the custom hook to clear the interval.
3.  Implemented React's `useCallback` and `useMemo` to prevent unnecessary re-renders.

**Prevention**: Added a linting rule (`exhaustive-deps`) to catch missing dependencies in `useEffect` and `useCallback` hooks.