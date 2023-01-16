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

<img src="Screenshots/logging_in.png" align="center" width="450px"/>
Logging in
<br/><br/>
<img src="Screenshots/registration.png" align="center" width="450px"/>
Registration
<br/><br/>
<img src="Screenshots/rooms_list.png" align="center" width="450px"/>
Rooms list
<br/><br/>
<img src="Screenshots/menu.png" align="center" width="450px"/>
Menu
<br/><br/>
<img src="Screenshots/room_info.png" align="center" width="450px"/>
Room info
<br/><br/>
<img src="Screenshots/booking.png" align="center" width="450px"/>
Room booking
<br/><br/>
<img src="Screenshots/customer_orders.png" align="center" width="450px"/>
Customer's orders
<br/><br/>
<img src="Screenshots/edit_data.png" align="center" width="450px"/>
Editing of user's personal data
<br/><br/>
<img src="Screenshots/all_orders.png" align="center" width="450px"/>
List of all orders
<br/><br/>
<img src="Screenshots/role_change.png" align="center" width="450px"/>
Changing of user's role
<br/><br/>