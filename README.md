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

<img src="Screenshots/logging_in.png" align="center" height="200px"/>
Logging in
<br clear="left"/>
<img src="Screenshots/Screenshots/registration.png" align="center" height="200px"/>
Registration
<br clear="left"/>
Rooms list
![Rooms list](Screenshots/rooms_list.png){float: none; height: 200px;}
Menu
![Menu](Screenshots/menu.png){float: none; height: 200px;}
Room info
![Room info](Screenshots/room_info.png){float: none; height: 200px;}
Room booking
![Room booking](Screenshots/booking.png){float: none; height: 200px;}
Customer's orders
![Customer's orders](Screenshots/customer_orders.png){float: none; height: 200px;}
Editing of user's personal data
![User data editing](Screenshots/edit_data.png){float: none; height: 200px;}
List of all orders
![All orders](Screenshots/all_orders.png){float: none; height: 200px;}
Changing of user's role
![Role changing](Screenshots/role_change.png){float: none; height: 200px;}
