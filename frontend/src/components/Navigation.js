import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import './Navigation.css';

function Navigation() {
  const location = useLocation();

  return (
    <nav className="navigation">
      <Link 
        to="/" 
        className={location.pathname === '/' ? 'nav-link active' : 'nav-link'}
      >
        Följ Användare
      </Link>
      <Link 
        to="/timeline" 
        className={location.pathname === '/timeline' ? 'nav-link active' : 'nav-link'}
      >
        Tidslinje
      </Link>
      <Link 
        to="/wall" 
        className={location.pathname === '/wall' ? 'nav-link active' : 'nav-link'}
      >
        Vägg
      </Link>
      <Link 
        to="/messages" 
        className={location.pathname === '/messages' ? 'nav-link active' : 'nav-link'}
      >
        Direktmeddelanden
      </Link>
      <Link 
        to="/create-post" 
        className={location.pathname === '/create-post' ? 'nav-link active' : 'nav-link'}
      >
        Skapa inlägg
      </Link>
    </nav>
  );
}

export default Navigation;
