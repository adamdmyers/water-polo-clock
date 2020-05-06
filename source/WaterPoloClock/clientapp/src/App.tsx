import React from 'react';
import './App.css';
import { BrowserRouter as Router, Switch, Route } from 'react-router-dom'
import ShotClock from './components/ShotClock'

function App() {
  return (
      <Router>
        <Switch>
          <Route path="/shotclock/:gameName"><ShotClock /></Route>
        </Switch>
      </Router>
  );
}

export default App;
