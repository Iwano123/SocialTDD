import React, { useState } from 'react';
import FollowUser from './components/FollowUser';
import FollowersList from './components/FollowersList';
import FollowingList from './components/FollowingList';
import Timeline from './components/Timeline';
import './App.css';

function App() {
  const [currentUserId, setCurrentUserId] = useState('');
  const [targetUserId, setTargetUserId] = useState('');
  const [refreshKey, setRefreshKey] = useState(0);

  const handleFollowChange = () => {
    setRefreshKey(prev => prev + 1);
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>SocialTDD</h1>
        <p>Följ och avfölj användare</p>
      </header>

      <main className="App-main">
        <div className="user-input-section">
          <div className="input-group">
            <label htmlFor="currentUserId">Ditt användar-ID (följare):</label>
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
          <div className="lists-section">
            <div className="lists-container">
              <FollowersList key={`followers-${refreshKey}`} userId={currentUserId} />
              <FollowingList key={`following-${refreshKey}`} userId={currentUserId} />
              <Timeline key={`timeline-${refreshKey}`} userId={currentUserId} />
            </div>
          </div>
        )}
      </main>
    </div>
  );
}

export default App;