export class ClaimValue {
    readonly type: string;
    readonly value: string;
    constructor(type: string, value: string) {
        this.type = type;
        this.value = value;
    }
}