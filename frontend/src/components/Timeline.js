import React, { useState, useEffect } from 'react';
import { postsApi } from '../services/postsApi';
import './Timeline.css';

function Timeline({ userId }) {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchTimeline = async () => {
      if (!userId) return;

      try {
        setLoading(true);
        setError(null);
        const data = await postsApi.getTimeline(userId);
        // Backend returnerar redan posts i kronologisk ordning (senaste först)
        setPosts(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchTimeline();
  }, [userId]);

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
        <div className="error">Fel: {error}</div>
      </div>
    );
  }

  return (
    <div className="timeline">
      <h3>Tidslinje ({posts.length})</h3>
      {posts.length === 0 ? (
        <div className="empty-message">Inga inlägg ännu</div>
      ) : (
        <div className="timeline-posts">
          {posts.map((post) => (
            <div key={post.id} className="post-item">
              <div className="post-header">
                <span className="post-sender">Från: {post.senderId}</span>
                <span className="post-date">
                  {new Date(post.createdAt).toLocaleString('sv-SE')}
                </span>
              </div>
              <div className="post-message">{post.message}</div>
              <div className="post-recipient">
                Till: {post.recipientId}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default Timeline;