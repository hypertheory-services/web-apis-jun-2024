

## Returning an Array:
GET /employees

```json
[
  {
    "id": "6d2957c4-5c1f-40fb-acc7-b54942a4686b",
    "firstName": "Joe",
    "lastName": "Gonzalez",
    "email": "joe@aol.com",
    "phone": "867-5309",
    "dateAdded": "2024-06-25T10:47:23.6656851-04:00",
    "addedBy": "sue@company.com"
  },
  {
    "id": "6797b1fe-9f5b-4458-a2f2-2b1442440d65",
    "firstName": "Sue",
    "lastName": "Gonzalez",
    "email": "sue@aol.com",
    "phone": "867-5309",
    "dateAdded": "2024-06-25T10:56:48.1923623-04:00",
    "addedBy": "sue@company.com"
  },
  {
    "id": "b488db93-4c58-4444-961b-399e4be14f5f",
    "firstName": "string",
    "lastName": "string",
    "email": "user@example.com",
    "phone": "string",
    "dateAdded": "2024-06-25T11:06:49.2314852-04:00",
    "addedBy": "boss@company.com"
  }
]
```


GET /employees?dept=DEV?page=2

```
{
    "numberOfFulltime": 15238 ,
    "numberOfContractors": 38739,
    "data": [
       ...100 of these (101-200) 
    ],
    "page": 2,
    "of": 837
}
```