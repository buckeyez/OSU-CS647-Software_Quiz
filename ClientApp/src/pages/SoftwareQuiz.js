import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import history from '../helpers/history'
// import "./SoftwareQuiz.css"
// import {Button} from 'react-bootstrap';


export default class SoftwareQuiz extends Component {
  // static displayName = Counter.name;

  constructor(props) {
    super(props);
    this.state = { quizID: [1] };
    this.incrementCounter = this.incrementCounter.bind(this);
  }

  incrementCounter() {
    this.setState({
      currentCount: this.state.currentCount + 1
    });
  }

  render() {
    return (
      <div>
        <h1>SoftwareQuiz - Create Quiz Main Page -Test</h1>
        <div id="createQuiz">
        <p>Add Questions to Question Pool</p>
        <form>
        <button id="addQuiz"
        onClick={() => history.push(   //THOUGHTS: should create a quizID as a central state
           '/new-quiz', "hiiii"
          )}>+</button>
        </form>
        
        </div>
        <div id="recentQuizes">
          <p>Recent Quizes</p>
        </div>
        {/* <p aria-live="polite">Current count: <strong>{this.state.currentCount}</strong></p>
        <button className="btn btn-primary" onClick={this.incrementCounter}>Increment</button> */}
      </div>
    );
  }
}