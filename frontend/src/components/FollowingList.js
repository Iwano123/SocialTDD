import React, { useState, useEffect } from 'react';
import { followApi } from '../services/followApi';
import './FollowingList.css';

function FollowingList({ userId }) {
  const [following, setFollowing] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchFollowing = async () => {
      if (!userId) return;

      try {
        setLoading(true);
        setError(null);
        const data = await followApi.getFollowing(userId);
        setFollowing(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchFollowing();
  }, [userId]);

  if (loading) {
    return (
      <div className="following-list">
        <h3>Följer</h3>
        <div className="loading">Laddar följda användare...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="following-list">
        <h3>Följer</h3>
        <div className="error">Fel: {error}</div>
      </div>
    );
  }

  return (
    <div className="following-list">
      <h3>Följer ({following.length})</h3>
      {following.length === 0 ? (
        <div className="empty-message">Följer inga användare ännu</div>
      ) : (
        <ul className="following-list-items">
          {following.map((follow) => (
            <li key={follow.id} className="following-item">
              <div className="following-info">
                <span className="following-id">Användare: {follow.followingId}</span>
                <span className="following-date">
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

export default FollowingList;