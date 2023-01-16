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

<img src="Screenshots/logging_in.png" align="center" height="450px"/>
Logging in
<br clear="left"/>

<img src="Screenshots/registration.png" align="center" height="450px"/>
Registration
<br clear="left"/>

<img src="Screenshots/rooms_list.png" align="center" height="450px"/>
Rooms list
<br clear="left"/>

<img src="Screenshots/menu.png" align="center" height="450px"/>
Menu
<br clear="left"/>

<img src="Screenshots/room_info.png" align="center" height="450px"/>
Room info
<br clear="left"/>

<img src="Screenshots/booking.png" align="center" height="450px"/>
Room booking
<br clear="left"/>

<img src="Screenshots/customer_orders.png" align="center" height="450px"/>
Customer's orders
<br clear="left"/>

<img src="Screenshots/edit_data.png" align="center" height="450px"/>
Editing of user's personal data
<br clear="left"/>

<img src="Screenshots/all_orders.png" align="center" height="450px"/>
List of all orders
<br clear="left"/>

<img src="Screenshots/role_change.png" align="center" height="450px"/>
Changing of user's role
<br clear="left"/>
