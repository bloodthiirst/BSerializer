# BSerializer

    - BSerializer is an attempt of making and understanding serializers in the process.

    - BSerializer is made using  *.NET Standard 2.0* so this should be usable for most C# projects. 

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

- where the parts surrounded by ```#``` are considered comment
- in the example above , the comments are auto-generated using the class's propertie names

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
