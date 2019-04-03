export default class TwitchComponentState {
    static from(state: Readonly<TwitchComponentState>): TwitchComponentState {
        const s = new TwitchComponentState();
        for (let prop in state) {
            s[prop] = state[prop];
        }
        return s;
    }
    
    [key: string]: any;
    public test: any = null;
    public username: any = null;
    public theme: string = "";
    public isVisible: boolean = true;
    public finishedLoading: boolean = false;
}
