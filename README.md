# Canvas-Stage

## Overview
Canvas & Stage is a comprehensive web-based platform designed to facilitate event management and artistic exhibitions. It empowers administrators to manage events, attendees, artists, artworks, and purchases, while also providing a dynamic space for artists to showcase their work and engage with audiences. The system seamlessly blends event coordination with creative expression, offering a smooth and interactive experience for both organizers and participants.

## Features
- **Event Management**: Create, update, and delete event records with ease.
- **Attendee Handling**: Maintain attendee details and manage participation efficiently.
- **Artist Profiles**: Register and manage artist information and exhibition involvement.
- **Artwork Exhibitions**: Showcase and manage a collection of artworks linked to specific artists and events.
- **Purchase Tracking**: Record and manage artwork purchases, providing a transactional overview.
- **Secure Admin Authentication**: Only authorized administrators can perform CRUD operations.
- **User-Friendly Interface**: Intuitive UI for efficient navigation and data interaction.

## Technologies Used
- **Backend**: ASP.NET Core
- **Frontend**: Razor Pages, HTML, CSS
- **Database**: SQL Server
- **Authentication**: Identity-based login system for secure admin access
- **Data Integrity**: Real-time entity relationships and integrity enforcement

## Challenges & Solutions
One of the core challenges was building a multi-entity CRUD system that accurately manages relationships between events, artists, and artworks without data conflicts. This was addressed by implementing:
- Relational mapping to enforce integrity between entities.
- Dynamic filtering of artists and artworks based on event context.
- Admin-only controls to restrict sensitive operations.

## Future Enhancements
- **Public Gallery View**: Allow public users to browse exhibitions and artist profiles.
- **Interactive Timelines**: Visual representation of event schedules and artist participation.
- **Notification System**: Send alerts for new events, exhibition updates, and purchases.
- **Reporting & Analytics**: Dashboard with engagement statistics and purchase insights.

## Installation & Setup
1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/canvas-and-stage.git
   ```
2. **Navigate to the project folder**
   ```bash
   cd canvas-and-stage
   ```
3. **Install dependencies**
   ```bash
   dotnet restore
   ```
4. **Set up the database (SQL Server)**
   - Update the connection string in `appsettings.json`
   - Run migrations to set up the database
   ```bash
   dotnet ef database update
   ```
5. **Run the application**
   ```bash
   dotnet run
   ```
6. **Open in browser:**
   - Navigate to `http://localhost:5000`

---
**Work Done By**:
1. Jinal Patel: Models, Page Controllers, Views and Styling, Extra Feature (Pagenation).
2. Isha Shah: API Controllers, Interfaces, Services and Readme File, Extra Feature (Purchase).
