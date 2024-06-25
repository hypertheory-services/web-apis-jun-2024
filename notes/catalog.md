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

Operands:
    - We need their name (first, last),
    - we need their email address
    - We need their phone number
    - we need their "identifier" (sub claim) (more on this later)


Resource: (an important thingy from the POV of our API, in the language of our "domain")

`/techs` - Identified by a "URI" (Uniform resource identifier), which is something like "https://api.company.com/software-catalog/techs"

There are different "kinds" of resources:

- Collections (a set (zero or more) of related resources) (/techs)
    - owns it's subordinate resources
    - part of that is it "decides" if something can be part of that collection.
    - We send it a "representation" of what we'd like, and it indicates failure or success.
    - If it is successful,  there will be a new document "subordinate" resource under the collection.
    
- Documents (a single thing, like /techs/39839893898938)
- Stores
- "Controllers" (confusing but I didn't come up with this)