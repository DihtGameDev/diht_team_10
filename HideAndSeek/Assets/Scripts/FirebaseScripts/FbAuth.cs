using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System;

public enum AuthStates {
    NULL, SUCCESS, CANCELED, IS_FAULTED
}

public enum FirebaseAuthErrorType {
    ALREADY_EXISTS, MISSING_PASSWORD, WEAK_PASSWORD, USER_NOT_FOUND, SOME_ERROR
}

public class AuthState {
    public AuthStates state = AuthStates.NULL;
}

public class FbAuth {
    private FirebaseAuth _auth;

    public FbAuth(FirebaseAuth auth) {
        _auth = auth;
    }

    public AuthState SignUpNewUser(string email, string password, Action<FirebaseUser> onSuccessAction = null, Action<FirebaseAuthErrorType> onFailAction = null) {
        AuthState authState = new AuthState();

        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled) {
                Debugger.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                authState.state = AuthStates.CANCELED;
                return;
            }
            if (task.IsFaulted) {
                onFailAction?.Invoke(GetErrorType(task.Exception));
                authState.state = AuthStates.IS_FAULTED;
                return;
            }

            // Firebase user has been created.
            FirebaseUser newUser = task.Result;

            onSuccessAction?.Invoke(newUser);  // if onSuccessAction not null

            authState.state = AuthStates.SUCCESS;
        });

        return authState;
    }

    public AuthState SignInExistingUser(string email, string password, Action<FirebaseUser> onSuccessAction = null, Action<FirebaseAuthErrorType> onFailAction = null) {
        AuthState authState = new AuthState();
        _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled) {
                authState.state = AuthStates.CANCELED;
                return;
            }
            if (task.IsFaulted) {
                onFailAction?.Invoke(GetErrorType(task.Exception));
                authState.state = AuthStates.IS_FAULTED;
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;

            onSuccessAction?.Invoke(newUser);  // if onSuccessAction not null

            authState.state = AuthStates.SUCCESS;
        });

        return authState;
    }

    public string MessageFromErrorType(FirebaseAuthErrorType errorType) {
        switch (errorType) {
            case FirebaseAuthErrorType.ALREADY_EXISTS: {
                return "this user already exists";
            }
            case FirebaseAuthErrorType.USER_NOT_FOUND: {
                return "user not found";
            }
            case FirebaseAuthErrorType.MISSING_PASSWORD: {
                return "missing password";
            }
            case FirebaseAuthErrorType.WEAK_PASSWORD: {
                return "password too weak";
            }
            case FirebaseAuthErrorType.SOME_ERROR: {
                return "something went wrong";
            }
            default: {
                return "Unknown error";
            }
        }
    }

    private FirebaseAuthErrorType GetErrorType(AggregateException exception) {
        if (exception == null) {
            return FirebaseAuthErrorType.SOME_ERROR;
        }

        FirebaseException fbException = exception.GetBaseException() as FirebaseException;
        AuthError authError = (AuthError)fbException.ErrorCode;

        Debugger.Log("AuthError to string: " + authError.ToString());

        switch (authError) {
            case AuthError.MissingPassword: {
                return FirebaseAuthErrorType.MISSING_PASSWORD;
            }
            case AuthError.WeakPassword: {
                return FirebaseAuthErrorType.WEAK_PASSWORD;
            }
            case AuthError.EmailAlreadyInUse: {
                return FirebaseAuthErrorType.ALREADY_EXISTS;
            }
            case AuthError.UserNotFound: {
                return FirebaseAuthErrorType.USER_NOT_FOUND;
            }
            default: {
                return FirebaseAuthErrorType.SOME_ERROR;
            }
        }
    }
}
