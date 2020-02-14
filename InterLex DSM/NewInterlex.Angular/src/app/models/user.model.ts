export class User {
  userName: string;
  accessToken: IAccessToken;
  refreshToken: string
}

export interface IAccessToken
{
  token:string;
  expiresIn: number;
}

