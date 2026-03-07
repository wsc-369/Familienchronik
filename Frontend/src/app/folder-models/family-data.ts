// family-data.ts

import { Person } from "../folder-interfaces/person ";

export const ROOT_PERSON: Person = {
  id: '1',
  name: 'Elternteil',
  partners: [
    { id: '2', name: 'Partnerin Anna' },
    { id: '3', name: 'Partnerin Clara' }
  ],
  children: [
    { id: '4', name: 'Kind A' },
    { id: '5', name: 'Kind B' },
    { id: '6', name: 'Kind C' }
  ]
};

export const ROOT_PERSONA: Person = {
  id: '1',
  name: 'Elternteil',
  partners: [
    {
      id: '2',
      name: 'Partnerin Anna',
      children: [
        { id: '3', name: 'Kind A' },
        { id: '4', name: 'Kind B' }
      ]
    },
    {
      id: '5',
      name: 'Partnerin Clara',
      children: [
        { id: '6', name: 'Kind C' }
      ]
    }
  ]
};

export const ROOT_PERSON2: Person = {
  id: '1',
  name: 'Elternteil',
  children: [
    { id: '2', name: 'Kind A' },
    { id: '3', name: 'Kind B' },
    { id: '4', name: 'Kind C' }
  ]
};


export const ROOT_PERSON1: Person = {
  id: '1',
  name: 'Großvater Karl',
  partners: [
    {
      id: '2',
      name: 'Partnerin Anna',
      children: [
        {
          id: '3',
          name: 'Kind Peter',
          partners: [
            {
              id: '4',
              name: 'Partnerin Julia',
              children: [
                { id: '5', name: 'Enkelin Lisa' },
                { id: '6', name: 'Enkel Max' }
              ]
            }
          ]
        },
        {
          id: '7',
          name: 'Kind Maria',
          children: [
            { id: '8', name: 'Enkelin Sophie' }
          ]
        }
      ]
    },
    {
      id: '9',
      name: 'Partnerin Clara',
      children: [
        {
          id: '10',
          name: 'Kind Thomas',
          partners: [
            {
              id: '11',
              name: 'Partnerin Eva',
              children: [
                { id: '12', name: 'Enkel Jonas' },
                { id: '13', name: 'Enkelin Mia' }
              ]
            }
          ]
        }
      ]
    }
  ]
};
