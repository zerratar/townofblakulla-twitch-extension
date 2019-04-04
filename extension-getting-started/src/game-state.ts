export default class GameState {
    static from(state: Readonly<GameState>): GameState {
        const s = new GameState();
        for (let prop in state) {
            s[prop] = state[prop];
        }
        return s;
    }
    
    [key: string]: any;
    public theme: string = "";
    public name: string = "";
    public joined: boolean = false;
    public state: number = -1;
    public isVisible: boolean = true;
    public finishedLoading: boolean = false;
}
