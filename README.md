# Synapse-Health Coding Challenge

## Running the Project

1. Clone the repository to your local machine.  
2. Navigate to the **`HandleOrders`** directory.  
3. Run the following command to execute the project in the development environment:  

   ```bash
   dotnet run dev
   ```
- **Important** because in dev we mock the API calls

## Running Unit Tests

1. Navigate to the **`HandleOrders.Tests`** directory
2. Execute the following command to run all unit tests

    ```bash
    dotnet test
    ```

## TODOs that could improve the project
- Implement DI service container
- add .env files and have configs for each env
- Add Exception handling, need more info from PO about how the flow should work

