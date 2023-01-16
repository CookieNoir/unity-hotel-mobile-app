# unity-hotel-mobile-app
Mobile application for booking rooms in hotel.

This app provides 3 roles for users: customer, receptionist, admin.

Customers can:
- register
- log in
- view rooms list
- view information about the selected room
- book a room
- manage their orders
- edit their personal data (username, phone number, password)

Receptionists in addition to the above can:
- view orders of all users and manage them
- add, edit or remove rooms

Admins in addition to the above can:
- change role of the selected user
- restore removed rooms

For password hashing I used the PBKDF2 algorithm.
For encrypting user phone numbers I used the AES block cipher algorithm.

Some screenshots:
Logging in
![Logging in](Screenshots/logging in.png)
Registration
![Registration](Screenshots/registration.png)
Rooms list
![Rooms list](Screenshots/rooms list.png)
Menu
![Menu](Screenshots/menu.png)
Room info
![Room info](Screenshots/room info.png)
Room booking
![Room booking](Screenshots/booking.png)
Customer's orders
![Customer's orders](Screenshots/customer orders.png)
Editing of user's personal data
![User data editing](Screenshots/edit data.png)
List of all orders
![All orders](Screenshots/all orders.png)
Changing of user's role
![Role changing](Screenshots/role change.png)
