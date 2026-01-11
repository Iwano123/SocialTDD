import React, { useState, useEffect, useCallback } from 'react';
import { wallApi } from '../services/wallApi';
import './Wall.css';

function Wall({ userId }) {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchWall = useCallback(async () => {
    if (!userId) {
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const wallPosts = await wallApi.getWall();
      // Sortera inlägg så att senaste kommer först (kronologisk ordning)
      const sortedPosts = wallPosts.sort((a, b) => {
        return new Date(b.createdAt) - new Date(a.createdAt);
      });
      setPosts(sortedPosts);
    } catch (err) {
      setError(err.message || 'Kunde inte hämta vägg');
    } finally {
      setLoading(false);
    }
  }, [userId]);

  useEffect(() => {
    if (userId) {
      fetchWall();
    }
  }, [userId, fetchWall]);

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('sv-SE', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  if (loading) {
    return (
      <div className="wall">
        <h2>Vägg</h2>
        <div className="loading">Laddar vägg...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="wall">
        <h2>Vägg</h2>
        <div className="error">
          {error}
          <button onClick={fetchWall} style={{ marginLeft: '10px', padding: '5px 10px' }}>
            Försök igen
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="wall">
      <h2>Vägg</h2>
      {posts.length === 0 ? (
        <div className="empty-message">
          <p>Inga inlägg från följda användare att visa.</p>
          <p className="empty-hint">Följ användare för att se deras inlägg här.</p>
        </div>
      ) : (
        <div className="posts-container">
          {posts.map((post) => (
            <div key={post.id} className="post-card">
              <div className="post-header">
                <span className="post-sender">Från: {post.senderId}</span>
                <span className="post-date">{formatDate(post.createdAt)}</span>
              </div>
              <div className="post-message">{post.message}</div>
              {post.recipientId && (
                <div className="post-footer">
                  <span className="post-recipient">Till: {post.recipientId}</span>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default Wall;
