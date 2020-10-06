# BSerializer

    - BSerializer is an attempt of making and understanding serializers in the process.

    - BSerializer is made using  **.NET Standard 2.0** so this should be usable for most C# projects. 

## Format

- The serialization output would be something like this :

### Serialization output
- One instance example
```
{
	<ConsoleUI.Model.Person>
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
		<ConsoleUI.Model.Person>
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
	<ConsoleUI.Model.Person>
	123,
	Bloodthirst,
	Ketsueki,
	Some place,
	{
		<ConsoleUI.Model.Person>
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
	<System.Collections.Generic.List`1[[ConsoleUI.Model.IPerson, ConsoleUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]>

	# [0] #
	{
		<ConsoleUI.Model.Person>
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
			<ConsoleUI.Model.Person>
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
		<ConsoleUI.Model.Person>
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