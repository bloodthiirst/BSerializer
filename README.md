# BSerializer

- BSerializer is an attempt of making and understanding serializers in the process.

- BSerializer is made using  **.NET Standard 2.0** so this should be usable for most C# projects. 

# Features implemented so far

- Support for recursive reference

- Auto-generated comments (with plans to make them custom)

- Caching optimization

## Examples:

for the next examples we will use this C# class to demonstrate the serializations result:

```csharp
namespace ConsoleUI.Model
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public Person Parent { get; set; }
    }
}
```

### Serialization output
- One instance example
```
{
	<ConsoleUI.Model.Person,0>
	# Id #
	123,
	# FirstName #
	Bloodthirst,
	# LastName #
	Ketsueki,
	# Address #
	Some place,
	# Parent #
	{
		<ConsoleUI.Model.Person,1>
		# Id #
		69,
		# FirstName #
		Parent,
		# LastName #
		McParenton,
		# Address #
		Some other place,
		# Parent #
		null
	}
}
```

the parts surrounded by ```#``` are considered comment
in the example above , the comments are auto-generated using the class's propertie names
with the auto-generated turned off , the result would look like this :

```
{
	<ConsoleUI.Model.Person,0>
	123,
	Bloodthirst,
	Ketsueki,
	Some place,
	{
		<ConsoleUI.Model.Person,1>
		69,
		Parent,
		McParenton,
		Some other place,
		null
	}
}
```

- List example
```
[
	<System.Collections.Generic.List`1[[ConsoleUI.Model.IPerson, ConsoleUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]] , 0>

	# [0] #
	{
		<ConsoleUI.Model.Person , 1>
		# Id #
		123,
		# FirstName #
		Bloodthirst,
		# LastName #
		Ketsueki,
		# Address #
		Some place,
		# Parent #
		{
			<ConsoleUI.Model.Person , 2>
			# Id #
			69,
			# FirstName #
			Parent,
			# LastName #
			McParenton,
			# Address #
			Some other place,
			# Parent #
			null
		}
	},

	# [1] #
	{
		<ConsoleUI.Model.Person , 2>
		# Id #
		69,
		# FirstName #
		Parent,
		# LastName #
		McParenton,
		# Address #
		Some other place,
		# Parent #
		null
	}
]
```
As you can see , with the auto-generated comments , a small comment indicating the index of the element is added above every elment of the list

- Dictionary example

```
[
	<System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e],[ConsoleUI.Model.Person, ConsoleUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]],0>

	# [0] #
	{
		# key #
		420,
		# value #
		{
			<ConsoleUI.Model.Person,1>
			# Id #
			69,
			# FirstName #
			Parent,
			# LastName #
			McParenton,
			# Address #
			Some other place,
			# Parent #
			{
				<ConsoleUI.Model.Person,1>
			},
			# age #
			32
		}
	},	

	# [1] #
	{
		# key #
		88,
		# value #
		{
			<ConsoleUI.Model.Person,1>
		}
	}
]
```
