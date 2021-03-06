import React, { useState, useContext } from 'react';
import { Form } from '../components';
import { UserContext } from '../context/userContext';
import { signup } from '../utils/signupUser';
import validator from 'validator';
import * as ROUTES from '../constants/routes';
import { useHistory } from 'react-router-dom';
import { storeUserSessionToLocalStorage } from '../utils/storeSession';
import {
  containerStyle,
  baseStyle,
  inputStyle,
  buttonStyle,
} from '../pages/styles/customPageStyles';

export default function Signup() {
  const history = useHistory();
  const [userName, setUserName] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [emailAddress, setEmailAddress] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const { /*user,*/ setUser } = useContext(UserContext);

  const isInvalid =
    userName === '' ||
    firstName === '' ||
    lastName === '' ||
    password === '' ||
    emailAddress === '';

  const handleSignUp = async (event) => {
    event.preventDefault();

    const isValidEmail = validator.isEmail(emailAddress);
    //validate password have at least 1 non alpha num char
    //validate password have at least 1 lowercase
    //validate password has at least 1 uppercase
    //ex jane: @$stonMartin007
    const isValidPassword = validator.isStrongPassword(password, {
      minLength: 6,
      minLowercase: 1,
      minUppercase: 1,
      minNumbers: 1,
      minSymbols: 1,
    });

    console.log(`valid email:${isValidEmail}, valid password:${isValidPassword}`);
    if (isValidEmail && isValidPassword) {
      const newUser = await signup(userName, emailAddress, password, lastName, firstName);
      if (newUser) {
        setUser(newUser);
        storeUserSessionToLocalStorage(newUser);
        history.push(ROUTES.HOME);
      }
    } else {
      setError(
        'A valid password must contain at least 1 non-alpha ,1 uppercase and 1 lowercase character '
      );
    }

    //code for sending new user to DB here
    console.log('new user signed up');
  };

  return (
    <div style={{ ...containerStyle }}>
      <Form>
        {error && <Form.Error>{error}</Form.Error>}
        <Form.Title>Sign up</Form.Title>
        <Form.Base onSubmit={handleSignUp} method="POST">
          <div style={{ ...baseStyle }}>
            <Form.Input
              style={{ ...inputStyle }}
              placeholder="User name"
              value={userName}
              onChange={({ target }) => setUserName(target.value.trim())}
            />

            <Form.Input
              style={{ ...inputStyle }}
              placeholder="First name"
              value={firstName}
              onChange={({ target }) => setFirstName(target.value.trim())}
            />

            <Form.Input
              style={{ ...inputStyle }}
              placeholder="Last name"
              value={lastName}
              onChange={({ target }) => setLastName(target.value.trim())}
            />

            <Form.Input
              style={{ ...inputStyle }}
              placeholder="Email address"
              value={emailAddress}
              onChange={({ target }) => setEmailAddress(target.value.trim())}
            />

            <Form.Input
              style={{ ...inputStyle }}
              type="password"
              placeholder="Password"
              autocomplete="off"
              value={password}
              onChange={({ target }) => setPassword(target.value.trim())}
            />
            <p style={{ fontSize: '12px' }}>
              A valid password must be 8 characters long, contain at least 1 non-alpha ,1 uppercase
              and 1 lowercase character
            </p>
            <Form.Submit style={{ ...buttonStyle }} disabled={isInvalid} type="submit">
              Sign Up
            </Form.Submit>
          </div>
        </Form.Base>
      </Form>
    </div>
  );
}
