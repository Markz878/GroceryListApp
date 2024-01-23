// Define a utility type that transforms the keys of an interface to capitalized keys
export type PascalCase<T> = {
    [K in keyof T as Capitalize<string & K>]: T[K];
};

