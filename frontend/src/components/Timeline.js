import React, { useState, useEffect, useCallback } from 'react';
import { postsApi } from '../services/postsApi';
import './Timeline.css';

function Timeline({ userId }) {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchTimeline = useCallback(async () => {
    if (!userId) {
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const timelinePosts = await postsApi.getTimeline();
      // Sortera inlägg så att senaste kommer först (kronologisk ordning)
      const sortedPosts = timelinePosts.sort((a, b) => {
        return new Date(b.createdAt) - new Date(a.createdAt);
      });
      setPosts(sortedPosts);
    } catch (err) {
      setError(err.message || 'Kunde inte hämta tidslinje');
    } finally {
      setLoading(false);
    }
  }, [userId]);

  useEffect(() => {
    if (userId) {
      fetchTimeline();
    }
  }, [userId, fetchTimeline]);

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
      <div className="timeline">
        <h3>Tidslinje</h3>
        <div className="loading">Laddar tidslinje...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="timeline">
        <h3>Tidslinje</h3>
        <div className="error">
          {error}
          <button onClick={fetchTimeline} style={{ marginLeft: '10px', padding: '5px 10px' }}>
            Försök igen
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="timeline">
      <h3>Tidslinje</h3>
      {posts.length === 0 ? (
        <div className="empty-message">
          Inga inlägg att visa i tidslinjen.
        </div>
      ) : (
        <div className="timeline-posts">
          {posts.map((post) => (
            <div key={post.id} className="post-item">
              <div className="post-header">
                <span className="post-sender">Från: {post.senderId}</span>
                <span className="post-date">{formatDate(post.createdAt)}</span>
              </div>
              <div className="post-message">{post.message}</div>
              {post.recipientId && (
                <div className="post-recipient">Till: {post.recipientId}</div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default Timeline;
