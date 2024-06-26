# Catalog

A item in the catalog has to be assigned to a tech that is the "owner" of that software.

The process of assigning a tech to own a piece of software is:

A software center manager assigns a tech to own the software.

However, THAT tech has to approve ownership before it is listed in the catalog.

```
h

```

## Techs

#### Operation: Create a Tech

Operands: - We need their name (first, last), - we need their email address - We need their phone number - we need their "identifier" (sub claim) (more on this later)

Resource: (an important thingy from the POV of our API, in the language of our "domain")

`/techs` - Identified by a "URI" (Uniform resource identifier), which is something like "https://api.company.com/software-catalog/techs"

There are different "kinds" of resources:

- Collections (a set (zero or more) of related resources) (/techs)
  - owns it's subordinate resources
  - part of that is it "decides" if something can be part of that collection.
  - We send it a "representation" of what we'd like, and it indicates failure or success.
  - If it is successful, there will be a new document "subordinate" resource under the collection.
- Documents (a single thing, like /techs/39839893898938)
- Stores
- "Controllers" (confusing but I didn't come up with this)

POST /techs/b0025560-22da-410b-aaae-fb4ffd1418f9/owned-software

GET /techs/b0025560-22da-410b-aaae-fb4ffd1418f9

```json
{
  "id": "6d2957c4-5c1f-40fb-acc7-b54942a4686b",
  "firstName": "Joe",
  "lastName": "Gonzalez",
  "email": "joe@aol.com",
  "phone": "867-5309",
}

GET /techs/b0025560-22da-410b-aaae-fb4ffd1418f9/owned-software


GET /catalog/{id}

{
    "id": 99,
    "title": "VsCode",
    "Description": "Editor",
    "owner": {
         "id": "6d2957c4-5c1f-40fb-acc7-b54942a4686b",
          "firstName": "Joe",
        "lastName": "Gonzalez",
        "email": "joe@aol.com",
        "phone": "867-5309",
    }
}


PUT /catalog/99
{
    "id": 99,
    "title": "Visual Studio Code",
    "Description": "Editor for developers",
    "owner": {
         "id": "6d2957c4-5c1f-40fb-acc7-b54942a4686b",
          "firstName": "Jorge",
        "lastName": "Gonzalez",
        "email": "joe@aol.com",
        "phone": "867-5309",
    }
}
```

What is the representation

New Software:

```json
{
  "id": "b0025560-22da-410b-aaae-fb4ffd1418f9",
  "title": "Jetbrains Rider",
  "description": ".NET IDE",
  "createdBy": "boss@company.com",
  "addedOn": "2024-06-25T15:04:37.095652-04:00"
}
```

What is this going to look like when it gets put in the catalog bucket?

GET /catalog/b0025560-22da-410b-aaae-fb4ffd1418f9

```json
{
  "id": "b0025560-22da-410b-aaae-fb4ffd1418f9",
  "title": "Jetbrains Rider",
  "_embedded": {
    "info": {
      "description": "An Ide For Developers"
    }
  },
  "_links": {
    "owner": "/techs/839839893"
  }
}
```

PUT /catalog/b0025560-22da-410b-aaae-fb4ffd1418f9/info

```json
{
  "description": "An Ide For Developers"
}
```


GET /employees/13

{
    "name": "Bob Smith",
    "email": "bob@company.com",
    "phone": "555-1212",
   
    "_links": {
        "manager": "/employees/1",
        "employee:salary": "/employees/13/salary"
    }
}


```json
{
  "id": "6d2957c4-5c1f-40fb-acc7-b54942a4686b",
  "firstName": "Joe",
  "lastName": "Gonzalez",
  "email": "joe@aol.com",
  "phone": "867-5309"
}
```

```json
{
  "id": "6d2957c4-5c1f-40fb-acc7-b54942a4686b",
  "firstName": "Joe",
  "lastName": "Gonzalez",
  "email": "joe@aol.com",
  "phone": "867-5309",
  "_links": {
    "tech:owned-software": "/techs/6d2957c4-5c1f-40fb-acc7-b54942a4686b/owned-software"
  }
}
```