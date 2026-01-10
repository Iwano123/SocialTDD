import React, { useState, useEffect } from 'react';
import { wallApi } from '../services/wallApi';
import './Wall.css';

function Wall({ userId }) {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchWall = async () => {
      if (!userId) {
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError(null);
        const data = await wallApi.getWall(userId);
        // Posts är redan sorterade kronologiskt (senaste först) från backend
        setPosts(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchWall();
  }, [userId]);

  if (!userId) {
    return (
      <div className="wall">
        <h2>Vägg</h2>
        <div className="empty-message">Ange ett användar-ID för att se din vägg</div>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="wall">
        <h2>Vägg</h2>
        <div className="loading">Laddar inlägg...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="wall">
        <h2>Vägg</h2>
        <div className="error">Fel: {error}</div>
      </div>
    );
  }

  return (
    <div className="wall">
      <h2>Vägg ({posts.length} inlägg)</h2>
      {posts.length === 0 ? (
        <div className="empty-message">
          <p>Inga inlägg ännu.</p>
          <p className="empty-hint">Följ användare för att se deras inlägg här!</p>
        </div>
      ) : (
        <div className="posts-container">
          {posts.map((post) => (
            <div key={post.id} className="post-card">
              <div className="post-header">
                <span className="post-sender">Från: {post.senderId}</span>
                <span className="post-date">
                  {new Date(post.createdAt).toLocaleString('sv-SE', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',
                  })}
                </span>
              </div>
              <div className="post-message">{post.message}</div>
              <div className="post-footer">
                <span className="post-recipient">Till: {post.recipientId}</span>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default Wall;