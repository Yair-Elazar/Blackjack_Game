# Blackjack_Game
A 2D Blackjack game built in Unity where a single player competes against a dealer. The game follows classic blackjack rules, allowing players to hit, stand, double down, and split hands.

🎮 Features

   ✅ Card deck system with shuffling and dealing logic

   ✅ Betting system with adjustable bet amounts

   ✅ Game actions: Hit, Stand, Double Down, and Split

   ✅ Split functionality if the player's first two cards are the same

   ✅ Persistent player data (money, wins, losses, total games played)

   ✅ Clean UI with dynamic updates

   ✅ Firebase integration for storing user data

Technologies Used:

   Unity (Game development)

   C# (Game logic and scripts)

   Firebase Firestore (Player data storage)

   Firebase Authentication (User login and data retrieval)

📥 Installation & Setup

  1. Clone the repository
  
    git clone https://github.com/ShlomoYalo/Blackjack_Game.git
    cd Blackjack_Game
  Open the project in Unity

  2 . Ensure you have Unity 2021+ installed
  
    Open the project from Unity Hub
    Set up Firebase (Optional, if using database features)

  3 . Create a Firebase project
  
    Enable Firestore for storing player data
    Download google-services.json and place it in Assets/StreamingAssets/

  4. Run the game
  
    Click Play in Unity Editor
    Build the game for Windows/macOS/WebGL

🕹️ How to Play

     1. Place a bet before starting a round

     2. Click Deal to receive your cards

    3. Choose an action:
    
      Hit → Draw another card.
      Stand → Keep your current hand.
      Double Down → Double your bet and receive one more card.
      Split → If you have two matching cards, split them into two hands.

  4. The dealer plays after your turn.

  5. The winner is determined based on classic blackjack rules.

  6. Your winnings are updated, and you can start a new round.
 
📄 License

  This project is open-source and available under the MIT License.

👨‍💻 Developer

  Created by ShlomoYalo


