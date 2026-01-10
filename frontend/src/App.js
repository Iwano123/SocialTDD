import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navigation from './components/Navigation';
import FollowUser from './components/FollowUser';
import FollowersList from './components/FollowersList';
import FollowingList from './components/FollowingList';
import Timeline from './components/Timeline';
import Wall from './components/Wall';
import DirectMessages from './components/DirectMessages';
import './App.css';

function App() {
  const [currentUserId, setCurrentUserId] = useState('');

  return (
    <Router>
      <div className="App">
        <header className="App-header">
          <h1>SocialTDD</h1>
          <p>Socialt nätverk</p>
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
          </div>

          {currentUserId && <Navigation />}

          <Routes>
            <Route 
              path="/" 
              element={
                currentUserId ? (
                  <FollowPage currentUserId={currentUserId} />
                ) : (
                  <div className="empty-state">Ange ditt användar-ID för att börja</div>
                )
              } 
            />
            <Route 
              path="/timeline" 
              element={
                currentUserId ? (
                  <Timeline userId={currentUserId} />
                ) : (
                  <div className="empty-state">Ange ditt användar-ID för att se tidslinje</div>
                )
              } 
            />
            <Route 
              path="/wall" 
              element={
                currentUserId ? (
                  <Wall userId={currentUserId} />
                ) : (
                  <div className="empty-state">Ange ditt användar-ID för att se vägg</div>
                )
              } 
            />
            <Route 
              path="/messages" 
              element={
                currentUserId ? (
                  <DirectMessages userId={currentUserId} />
                ) : (
                  <div className="empty-state">Ange ditt användar-ID för att se meddelanden</div>
                )
              } 
            />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

// Komponent för Följ-sidan
function FollowPage({ currentUserId }) {
  const [targetUserId, setTargetUserId] = useState('');
  const [refreshKey, setRefreshKey] = useState(0);

  const handleFollowChange = () => {
    setRefreshKey(prev => prev + 1);
  };

  return (
    <div>
      <div className="user-input-section">
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

        {targetUserId && (
          <div className="follow-section">
            <FollowUser
              followerId={currentUserId}
              followingId={targetUserId}
              onFollowChange={handleFollowChange}
            />
          </div>
        )}
      </div>

      <div className="lists-section">
        <div className="lists-container">
          <FollowersList key={`followers-${refreshKey}`} userId={currentUserId} />
          <FollowingList key={`following-${refreshKey}`} userId={currentUserId} />
        </div>
      </div>
    </div>
  );
}

export default App;