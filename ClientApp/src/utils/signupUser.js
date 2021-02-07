import axios from 'axios';

/**
 * Gets user object from /users/signin endpoint
 * @param  {string} emailAddress email address of user
 * @param  {string} password password of user
 * @return {object}      user object containing id,name, firstName, lastName, email
 */
export const signup = async (username, emailAddress, password, lastName, firstName) => {
  //Required data for reqeust body
  const data = {
    name: username,
    email: emailAddress,
    password: password,
    lastName: lastName,
    firstName: firstName,
  };
  const headers = {
    'Access-Control-Allow-Origin': 'https://localhost:5001/',
  };

  const response = await axios
    .post('https://localhost:5001/users/add', data, headers)
    .catch((e) => console.log(e));

  //If user exists, return user object, else undefined
  if (response) {
    console.log(response);
    return response.data;
  } else {
    return undefined;
  }
};
