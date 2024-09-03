import React from 'react';
import EmptyFilter from '../../../components/EmptyFilter';

export default function SignIn({
	searchParams,
}: {
	searchParams: { callbackUrl: string };
}) {
	return (
		<EmptyFilter
			title='You need to to be logged in to do that'
			subtitle='Please click below to login'
			showLogin
			callbackUrl={searchParams.callbackUrl}
		/>
	);
}
