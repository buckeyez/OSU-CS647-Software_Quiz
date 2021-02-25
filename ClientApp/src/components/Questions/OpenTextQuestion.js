import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { TransitionPropTypeKeys } from 'reactstrap/lib/utils';
// import history from './history'
// import "./SoftwareQuiz.css"
// import {Button} from 'react-bootstrap';

// import "./QuestionTemplate.css"

const OpenTextQuestion = (props) => {
  return (
    <div>
      <input
        type="text"
        value={props.answers[0] ? props.answers[0].value : ''}
        onChange={props.handleOpenTextAnswer}
      />
    </div>
  );
};

export default OpenTextQuestion;
