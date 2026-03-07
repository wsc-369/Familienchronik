// person.model.ts
export interface Person {
  id: string;
  name: string;
  partners?: Person[];
  children?: Person[];
}
