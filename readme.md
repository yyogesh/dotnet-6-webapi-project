dotnet tool install --global dotnet-ef

Microsoft.EntityFramework.Design

dotnet ef dbcontext scaffold "Server=localhost\SQLEXPRESS;Database=Learn;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models