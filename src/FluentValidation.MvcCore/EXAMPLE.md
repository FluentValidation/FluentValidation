### Example
```csharp
using FluentValidation;

public class Startup {
   ...
  
   public void ConfigureServices(IServiceCollection services) {

            services.AddMvc()
                    .AddFluentValidation(Assembly.GetEntryAssembly());
   }
}

```