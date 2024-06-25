# Adding Software

After reflection, and "E"'s comment about "workflow", I decided that we are going to do this "right". And my original business story was too authoritarian.

## User Stories

### An Software Center Admin Approves A Piece of Software 

A member of the SoftwareCenter Admin role can add a piece of software.

The operands are:
- The sub of the admin making the request.
- The title of the software (Must be unique)
- A description of the software


## Assigning an Owner

Any SoftwareCenter Tech can see a list of approved software from the Admins (see above) that has not yet been assigned an owner.
A Tech can take ownership of this piece of software.

Rules:
Each piece of software can only have one owner.
After an owner claims the software it should "appear" as a resource in the catalog.
A tech can own multiple pieces of software.
