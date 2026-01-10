import React, { useState } from 'react';
import FollowUser from './components/FollowUser';
import FollowersList from './components/FollowersList';
import FollowingList from './components/FollowingList';
import Wall from './components/Wall';
import './App.css';

function App() {
  const [currentUserId, setCurrentUserId] = useState('');
  const [targetUserId, setTargetUserId] = useState('');
  const [refreshKey, setRefreshKey] = useState(0);

  const handleFollowChange = () => {
    // Uppdatera listorna och väggen när följ-status ändras
    setRefreshKey(prev => prev + 1);
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>SocialTDD</h1>
        <p>Följ användare och se deras inlägg</p>
      </header>

      <main className="App-main">
        <div className="user-input-section">
          <div className="input-group">
            <label htmlFor="currentUserId">Ditt användar-ID:</label>
            <input
              id="currentUserId"
              type="text"
              value={currentUserId}
              onChange={(e) => setCurrentUserId(e.target.value)}
              placeholder="Ange ditt användar-ID"
              className="user-input"
            />
          </div>

          <div className="input-group">
            <label htmlFor="targetUserId">Användar-ID att följa:</label>
            <input
              id="targetUserId"
              type="text"
              value={targetUserId}
              onChange={(e) => setTargetUserId(e.target.value)}
              placeholder="Ange användar-ID att följa"
              className="user-input"
            />
          </div>

          {currentUserId && targetUserId && (
            <div className="follow-section">
              <FollowUser
                followerId={currentUserId}
                followingId={targetUserId}
                onFollowChange={handleFollowChange}
              />
            </div>
          )}
        </div>

        {currentUserId && (
          <div className="content-section">
            <div className="wall-section">
              <Wall key={`wall-${ refreshKey }`} userId={currentUserId} />
            </div>
            <div className="lists-section">
              <div className="lists-container">
                <FollowersList key={`followers-${ refreshKey }`} userId={currentUserId} />
                <FollowingList key={`following-${ refreshKey }`} userId={currentUserId} />
              </div>
            </div>
          </div>
        )}
      </main>
    </div>
  );
}

export default App;