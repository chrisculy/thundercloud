import { Resource } from "./Resource";

export type ResourceSet = {
  [resource in Resource]?: number;
};

export const resourceSetToStrings = (resources: ResourceSet): string[] => {
  const result = [];
  if (resources.wood) {
    result.push(`${resources.wood} wood`);
  }
  if (resources.stone) {
    result.push(`${resources.stone} stone`);
  }
  if (resources.food) {
    result.push(`${resources.food} food`);
  }
  if (resources.superfood) {
    result.push(`${resources.superfood} superfood`);
  }
  if (resources.knowledge) {
    result.push(`${resources.knowledge} knowledge`);
  }
  if (resources.stormknowledge) {
    result.push(`${resources.stormknowledge} stormknowledge`);
  }
  if (resources.power) {
    result.push(`${resources.power} power`);
  }
  if (resources.stormpower) {
    result.push(`${resources.stormpower} stormpower`);
  }

  return result;
}