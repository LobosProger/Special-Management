# Special-Management
### Optimized Spatial Interest Management Component from the Mirror Networking Library

* A flexible component for interest management
* Much more efficient, as the built-in component checks every entity within 9 cells when searching for players. In contrast, each entity in this component checks its own cell, where it is located according to a player-filled dictionary, defining the "visibility zones."

### Results:
Built-in component in benchmark of 1000 entities takes 4.73 milliseconds on processing:

![photo1704648751](https://github.com/LobosProger/Special-Management/assets/78168123/174ab5be-e797-4c94-8d7b-d949f0e9dcc6)

Optimized version in benchmark of 1000 entities takes 4.73 milliseconds on processing (optimized on 43 %):

![photo1704648751 (1)](https://github.com/LobosProger/Special-Management/assets/78168123/e16828f0-5827-4cf6-a1d9-c3d67e7773ca)

* After expressing my intent to sell the optimized implementation on the developer forum, an enthusiast working on his own network library contacted me. 
* We negotiated a price of $45, and I successfully sold my enhanced implementation to him:

![image](https://github.com/LobosProger/Special-Management/assets/78168123/41767d2b-5fad-45bb-963a-15354d4a6a32)
