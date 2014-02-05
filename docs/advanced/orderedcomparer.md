# OrderedComparer

OrderedComparer is a composer that allows you to create a comparer using the familiar LINQ ordering syntax. It's handy for creating a comparer for use in derived collections.

## Lets sort some employees

Say that we have a ```ReactiveList<Employee>``` where ```Employee``` is defined as follows.

```csharp
public class Employee
{
    public string Name {get; set;}
	public string Department {get; set;}
    public double Salary {get; set;}
}
```

Seeing as it's such an easy model let's try to write a comparison method that sorts all employees by their department, then by their Name and finally by their salary (descending).

```csharp
public int SortEmployee(Employee x, Employee y) 
{
    int department = x.Department.CompareTo(y.Department);
    if(department != 0) return 0;

    int name = x.Name.CompareTo(y.Name);
    if(name != 0) return 0;

    // Swap x and y for descending
    return y.Salary.CompareTo(x.Salary)
}
```

Not too bad but what happens if we have an employee without a department?```NullReferenceException``` is what happens. And there's more edge cases like this and you can imagine this method being way more complex and error-prone with a more complex model. Let's try write the equivalent using an ```OrderedComparer``` instead.

```csharp

readonly static IComparer<Employee> employeeComparer = OrderedComparer<Employee>
    .OrderBy(x => x.Department)
    .ThenBy(x => x.Name)
    .ThenByDescending(x => x.Salary);

public int SortEmployee(Employee x, Employee y) 
{
    return employeeComparer.Compare(x, y);
}
```

## Use in CreateDerivedCollection

CreateDerivedCollection unfortunately doesn't accept an IComparer<Employee>, it instead takes a ```Comparison<Employee>``` delegate. Luckily that's exactly what ```IComparer<Employee>.Compare``` is.

```csharp
   var employees = new ReactiveList<Employee> { ... }
   var orderedEmployees = employees.CreateDerivedCollect(
       x => x, 
       orderer: OrderedComparer<Employee>
           .OrderBy(x => x.Department)
           .ThenBy(x => x.Name)
           .ThenByDescending(x => x.Salary).Compare; // .Compare on the last
   );
```

Testing this is now a matter of putting some employees into the list and verifying that orderedEmployees matches the order you want it to. If you use the example
above of creating a dedicated Sort method you can even just pass that straight in.

```csharp
   var employees = new ReactiveList<Employee> { ... }
   var orderedEmployees = employees.CreateDerivedCollect(x => x, orderer:  SortEmployee);
```