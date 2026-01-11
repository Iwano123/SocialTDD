import React, { useState, useEffect } from 'react';
import { followApi } from '../services/followApi';
import { userApi } from '../services/userApi';
import './FollowersList.css';

function FollowersList({ userId }) {
  const [followers, setFollowers] = useState([]);
  const [usernames, setUsernames] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchFollowers = async () => {
      if (!userId) return;

      try {
        setLoading(true);
        setError(null);
        const data = await followApi.getFollowers(userId);
        setFollowers(data);

        // Hämta användarnamn för alla följare
        const usernameMap = {};
        await Promise.all(
          data.map(async (follow) => {
            try {
              const user = await userApi.getUserById(follow.followerId);
              if (user) {
                usernameMap[follow.followerId] = user.username;
              }
            } catch (err) {
              console.error(`Kunde inte hämta användare ${follow.followerId}:`, err);
            }
          })
        );
        setUsernames(usernameMap);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchFollowers();
  }, [userId]);

  if (loading) {
    return (
      <div className="followers-list">
        <h3>Följare</h3>
        <div className="loading">Laddar följare...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="followers-list">
        <h3>Följare</h3>
        <div className="error">Fel: {error}</div>
      </div>
    );
  }

  return (
    <div className="followers-list">
      <h3>Följare ({followers.length})</h3>
      {followers.length === 0 ? (
        <div className="empty-message">Inga följare ännu</div>
      ) : (
        <ul className="followers-list-items">
          {followers.map((follow) => (
            <li key={follow.id} className="follower-item">
              <div className="follower-info">
                <span className="follower-id">
                  {usernames[follow.followerId] || follow.followerId}
                </span>
                <span className="follower-date">
                  Följer sedan: {new Date(follow.createdAt).toLocaleDateString('sv-SE')}
                </span>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

export default FollowersList;